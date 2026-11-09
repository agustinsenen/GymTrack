using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using GymTrack.API.Data;
using GymTrack.API.DTOs.Ejercicio;
using GymTrack.API.Exceptions;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Services
{
    public class EjercicioService : IEjercicioService
    {
        private const string CarpetaFotos = "ejercicios";

        private readonly AppDbContext _context;
        private readonly IAuditoriaService _auditoria;
        private readonly IImagenService _imagenes;

        public EjercicioService(AppDbContext context, IAuditoriaService auditoria, IImagenService imagenes)
        {
            _context = context;
            _auditoria = auditoria;
            _imagenes = imagenes;
        }

        private static readonly Expression<Func<Ejercicio, EjercicioDto>> AEjercicioDto = e => new EjercicioDto
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Descripcion = e.Descripcion,
            Observacion = e.Observacion,
            FotoUrl = e.FotoUrl,
            VideoUrl = e.VideoUrl,
            GrupoMuscularId = e.GrupoMuscularId,
            GrupoMuscularNombre = e.GrupoMuscular!.Nombre,
            Activo = e.Activo
        };

        public async Task<List<GrupoMuscularDto>> ListarGruposAsync()
        {
            return await _context.GruposMusculares
                .OrderBy(g => g.Nombre)
                .Select(g => new GrupoMuscularDto { Id = g.Id, Nombre = g.Nombre })
                .ToListAsync();
        }

        public async Task<List<EjercicioDto>> ListarAsync(int? grupoMuscularId, string? buscar, bool incluirInactivos)
        {
            var query = _context.Ejercicios.AsQueryable();

            if (!incluirInactivos)
            {
                query = query.Where(e => e.Activo);
            }

            if (grupoMuscularId.HasValue)
            {
                query = query.Where(e => e.GrupoMuscularId == grupoMuscularId.Value);
            }

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var texto = buscar.Trim();
                query = query.Where(e => e.Nombre.Contains(texto));
            }

            return await query
                .OrderBy(e => e.Nombre)
                .Select(AEjercicioDto)
                .ToListAsync();
        }

        public async Task<EjercicioDto> ObtenerAsync(int id)
        {
            return await _context.Ejercicios
                .Where(e => e.Id == id)
                .Select(AEjercicioDto)
                .FirstOrDefaultAsync()
                ?? throw new NoEncontradoException("El ejercicio no existe.");
        }

        public async Task<EjercicioDto> CrearAsync(CrearEjercicioDto dto, int usuarioId)
        {
            await ValidarAsync(dto, null);

            var ejercicio = new Ejercicio
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = Limpiar(dto.Descripcion),
                Observacion = Limpiar(dto.Observacion),
                VideoUrl = Limpiar(dto.VideoUrl),
                GrupoMuscularId = dto.GrupoMuscularId
            };

            if (dto.Foto != null)
            {
                ejercicio.FotoUrl = await _imagenes.GuardarAsync(dto.Foto, CarpetaFotos);
            }

            _context.Ejercicios.Add(ejercicio);
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("CREAR", "Ejercicio", ejercicio.Id, usuarioId, ejercicio.Nombre);

            return await ObtenerAsync(ejercicio.Id);
        }

        public async Task<EjercicioDto> ActualizarAsync(int id, ActualizarEjercicioDto dto, int usuarioId)
        {
            var ejercicio = await _context.Ejercicios.FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new NoEncontradoException("El ejercicio no existe.");

            await ValidarAsync(dto, id);

            ejercicio.Nombre = dto.Nombre.Trim();
            ejercicio.Descripcion = Limpiar(dto.Descripcion);
            ejercicio.Observacion = Limpiar(dto.Observacion);
            ejercicio.VideoUrl = Limpiar(dto.VideoUrl);
            ejercicio.GrupoMuscularId = dto.GrupoMuscularId;
            ejercicio.Activo = dto.Activo;

            if (dto.Foto != null)
            {
                // Foto nueva: se guarda y se borra la anterior
                var fotoAnterior = ejercicio.FotoUrl;
                ejercicio.FotoUrl = await _imagenes.GuardarAsync(dto.Foto, CarpetaFotos);
                _imagenes.Eliminar(fotoAnterior);
            }
            else if (dto.QuitarFoto)
            {
                _imagenes.Eliminar(ejercicio.FotoUrl);
                ejercicio.FotoUrl = null;
            }

            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("ACTUALIZAR", "Ejercicio", id, usuarioId, ejercicio.Nombre);

            return await ObtenerAsync(id);
        }

        public async Task DarDeBajaAsync(int id, int usuarioId)
        {
            var ejercicio = await _context.Ejercicios.FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new NoEncontradoException("El ejercicio no existe.");

            // Baja lógica: puede estar en rutinas o en el historial de progreso
            ejercicio.Activo = false;

            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("BAJA", "Ejercicio", id, usuarioId, ejercicio.Nombre);
        }

        private async Task ValidarAsync(CrearEjercicioDto dto, int? idActual)
        {
            if (!await _context.GruposMusculares.AnyAsync(g => g.Id == dto.GrupoMuscularId))
            {
                throw new ReglaDeNegocioException("El grupo muscular indicado no existe.");
            }

            var nombre = dto.Nombre.Trim();

            if (await _context.Ejercicios.AnyAsync(e => e.Nombre == nombre && e.Id != idActual))
            {
                throw new ReglaDeNegocioException("Ya existe un ejercicio con ese nombre.");
            }
        }

        private static string? Limpiar(string? texto) =>
            string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }
}
