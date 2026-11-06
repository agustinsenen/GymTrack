using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using GymTrack.API.Data;
using GymTrack.API.DTOs.Alumno;
using GymTrack.API.Exceptions;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Services
{
    public class AlumnoService : IAlumnoService
    {
        private readonly AppDbContext _context;
        private readonly IAuditoriaService _auditoria;

        public AlumnoService(AppDbContext context, IAuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        private static readonly Expression<Func<Alumno, AlumnoDto>> AAlumnoDto = a => new AlumnoDto
        {
            Id = a.Id,
            UsuarioId = a.UsuarioId,
            Nombre = a.Usuario!.Nombre,
            Email = a.Usuario.Email,
            Dni = a.Dni,
            ProfesorId = a.ProfesorId,
            ProfesorNombre = a.Profesor!.Nombre,
            Activo = a.Usuario.Activo,
            // El vencimiento vigente es el del último pago
            FechaVencimientoCuota = a.Pagos.Max(p => (DateTime?)p.NuevaFechaVencimiento)
        };

        // Calcula los días que faltan y si está vencida (se hace en C#, no en SQL)
        private static AlumnoDto CompletarEstadoCuota(AlumnoDto dto)
        {
            if (dto.FechaVencimientoCuota == null)
            {
                dto.DiasParaVencimiento = null;
                dto.CuotaVencida = true;
            }
            else
            {
                dto.DiasParaVencimiento = (dto.FechaVencimientoCuota.Value.Date - FechaHelper.Hoy).Days;
                dto.CuotaVencida = dto.DiasParaVencimiento < 0;
            }

            return dto;
        }

        private IQueryable<Alumno> AlumnosVisibles(int usuarioActualId, bool esAdmin)
        {
            var query = _context.Alumnos.AsQueryable();

            if (!esAdmin)
            {
                query = query.Where(a => a.ProfesorId == usuarioActualId);
            }

            return query;
        }

        public async Task<List<AlumnoDto>> ListarAsync(int usuarioActualId, bool esAdmin, string? buscar, bool incluirInactivos)
        {
            var query = AlumnosVisibles(usuarioActualId, esAdmin);

            if (!incluirInactivos)
            {
                query = query.Where(a => a.Usuario!.Activo);
            }

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var texto = buscar.Trim();
                query = query.Where(a => a.Usuario!.Nombre.Contains(texto) || a.Dni.Contains(texto));
            }

            var alumnos = await query
                .OrderBy(a => a.Usuario!.Nombre)
                .Select(AAlumnoDto)
                .ToListAsync();

            return alumnos.Select(CompletarEstadoCuota).ToList();
        }

        public async Task<AlumnoDto> ObtenerAsync(int id, int usuarioActualId, bool esAdmin)
        {
            await VerificarAccesoAsync(id, usuarioActualId, esAdmin);

            return await ObtenerPropioAsync(id);
        }

        public async Task<AlumnoDto> ObtenerPropioAsync(int alumnoId)
        {
            var alumno = await _context.Alumnos
                .Where(a => a.Id == alumnoId)
                .Select(AAlumnoDto)
                .FirstOrDefaultAsync()
                ?? throw new NoEncontradoException("El alumno no existe.");

            return CompletarEstadoCuota(alumno);
        }

        public async Task<AlumnoDto> CrearAsync(CrearAlumnoDto dto, int usuarioActualId, bool esAdmin)
        {
            int profesorId;

            if (esAdmin)
            {
                profesorId = dto.ProfesorId
                    ?? throw new ReglaDeNegocioException("Indicá el profesor a cargo del alumno.");

                await ValidarProfesorAsync(profesorId);
            }
            else
            {
                // Si lo crea un profesor, el alumno queda a su cargo
                profesorId = usuarioActualId;
            }

            var email = dto.Email.Trim().ToLower();
            var dni = dto.Dni.Trim();

            if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            {
                throw new ReglaDeNegocioException("Ya existe un usuario con ese email.");
            }

            if (await _context.Alumnos.AnyAsync(a => a.Dni == dni))
            {
                throw new ReglaDeNegocioException("Ya existe un alumno con ese DNI.");
            }

            var rolId = await _context.Roles
                .Where(r => r.Nombre == Rol.Alumno)
                .Select(r => r.Id)
                .FirstAsync();

            var usuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Email = email,
                // Si el profesor no carga contraseña, la inicial es el DNI
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    string.IsNullOrWhiteSpace(dto.Password) ? dni : dto.Password),
                RolId = rolId,
                Alumno = new Alumno
                {
                    Dni = dni,
                    ProfesorId = profesorId
                }
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var alumnoId = usuario.Alumno!.Id;

            await _auditoria.RegistrarAsync("CREAR", "Alumno", alumnoId, usuarioActualId,
                $"{usuario.Nombre} (DNI {dni})");

            return await ObtenerPropioAsync(alumnoId);
        }

        public async Task<AlumnoDto> ActualizarAsync(int id, ActualizarAlumnoDto dto, int usuarioActualId, bool esAdmin)
        {
            await VerificarAccesoAsync(id, usuarioActualId, esAdmin);

            var alumno = await _context.Alumnos
                .Include(a => a.Usuario)
                .FirstAsync(a => a.Id == id);

            var email = dto.Email.Trim().ToLower();
            var dni = dto.Dni.Trim();

            if (await _context.Usuarios.AnyAsync(u => u.Email == email && u.Id != alumno.UsuarioId))
            {
                throw new ReglaDeNegocioException("Ya existe otro usuario con ese email.");
            }

            if (await _context.Alumnos.AnyAsync(a => a.Dni == dni && a.Id != id))
            {
                throw new ReglaDeNegocioException("Ya existe otro alumno con ese DNI.");
            }

            alumno.Dni = dni;
            alumno.Usuario!.Nombre = dto.Nombre.Trim();
            alumno.Usuario.Email = email;
            alumno.Usuario.Activo = dto.Activo;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                alumno.Usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("ACTUALIZAR", "Alumno", id, usuarioActualId, alumno.Usuario.Nombre);

            return await ObtenerPropioAsync(id);
        }

        public async Task DarDeBajaAsync(int id, int usuarioActualId, bool esAdmin)
        {
            await VerificarAccesoAsync(id, usuarioActualId, esAdmin);

            var alumno = await _context.Alumnos
                .Include(a => a.Usuario)
                .FirstAsync(a => a.Id == id);

            // Baja lógica: se conserva su historial de pagos, rutinas y progreso
            alumno.Usuario!.Activo = false;

            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("BAJA", "Alumno", id, usuarioActualId, alumno.Usuario.Nombre);
        }

        public async Task<AlumnoDto> ReasignarProfesorAsync(int id, int profesorId, int adminId)
        {
            var alumno = await _context.Alumnos.FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new NoEncontradoException("El alumno no existe.");

            await ValidarProfesorAsync(profesorId);

            var profesorAnterior = alumno.ProfesorId;
            alumno.ProfesorId = profesorId;

            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("REASIGNAR", "Alumno", id, adminId,
                $"Profesor {profesorAnterior} -> {profesorId}");

            return await ObtenerPropioAsync(id);
        }

        public async Task VerificarAccesoAsync(int alumnoId, int usuarioActualId, bool esAdmin)
        {
            var profesorId = await _context.Alumnos
                .Where(a => a.Id == alumnoId)
                .Select(a => (int?)a.ProfesorId)
                .FirstOrDefaultAsync();

            if (profesorId == null)
            {
                throw new NoEncontradoException("El alumno no existe.");
            }

            if (!esAdmin && profesorId != usuarioActualId)
            {
                throw new AccesoDenegadoException("El alumno no está a tu cargo.");
            }
        }

        private async Task ValidarProfesorAsync(int profesorId)
        {
            var existe = await _context.Usuarios.AnyAsync(u =>
                u.Id == profesorId &&
                u.Activo &&
                u.Rol!.Nombre == Rol.Profesor);

            if (!existe)
            {
                throw new ReglaDeNegocioException("El profesor indicado no existe o está dado de baja.");
            }
        }
    }
}
