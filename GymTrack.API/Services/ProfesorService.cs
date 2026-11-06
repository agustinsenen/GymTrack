using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using GymTrack.API.Data;
using GymTrack.API.DTOs.Profesor;
using GymTrack.API.Exceptions;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Services
{
    public class ProfesorService : IProfesorService
    {
        private readonly AppDbContext _context;
        private readonly IAuditoriaService _auditoria;

        public ProfesorService(AppDbContext context, IAuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        private static readonly Expression<Func<Usuario, ProfesorDto>> AProfesorDto = u => new ProfesorDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Email = u.Email,
            Activo = u.Activo,
            CantidadAlumnos = u.AlumnosACargo.Count(a => a.Usuario!.Activo)
        };

        private IQueryable<Usuario> Profesores() =>
            _context.Usuarios.Where(u => u.Rol!.Nombre == Rol.Profesor);

        public async Task<List<ProfesorDto>> ListarAsync(bool incluirInactivos)
        {
            var query = Profesores();

            if (!incluirInactivos)
            {
                query = query.Where(u => u.Activo);
            }

            return await query
                .OrderBy(u => u.Nombre)
                .Select(AProfesorDto)
                .ToListAsync();
        }

        public async Task<ProfesorDto> ObtenerAsync(int id)
        {
            return await Profesores()
                .Where(u => u.Id == id)
                .Select(AProfesorDto)
                .FirstOrDefaultAsync()
                ?? throw new NoEncontradoException("El profesor no existe.");
        }

        public async Task<ProfesorDto> CrearAsync(CrearProfesorDto dto, int adminId)
        {
            var email = dto.Email.Trim().ToLower();

            if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            {
                throw new ReglaDeNegocioException("Ya existe un usuario con ese email.");
            }

            var rolId = await _context.Roles
                .Where(r => r.Nombre == Rol.Profesor)
                .Select(r => r.Id)
                .FirstAsync();

            var profesor = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RolId = rolId
            };

            _context.Usuarios.Add(profesor);
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("CREAR", "Profesor", profesor.Id, adminId, profesor.Nombre);

            return await ObtenerAsync(profesor.Id);
        }

        public async Task<ProfesorDto> ActualizarAsync(int id, ActualizarProfesorDto dto, int adminId)
        {
            var profesor = await Profesores().FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new NoEncontradoException("El profesor no existe.");

            var email = dto.Email.Trim().ToLower();

            if (await _context.Usuarios.AnyAsync(u => u.Email == email && u.Id != id))
            {
                throw new ReglaDeNegocioException("Ya existe otro usuario con ese email.");
            }

            profesor.Nombre = dto.Nombre.Trim();
            profesor.Email = email;
            profesor.Activo = dto.Activo;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                profesor.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("ACTUALIZAR", "Profesor", id, adminId, profesor.Nombre);

            return await ObtenerAsync(id);
        }

        public async Task DarDeBajaAsync(int id, int adminId)
        {
            var profesor = await Profesores().FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new NoEncontradoException("El profesor no existe.");

            // Baja lógica: sus alumnos siguen existiendo y el admin los puede reasignar
            profesor.Activo = false;

            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("BAJA", "Profesor", id, adminId, profesor.Nombre);
        }
    }
}
