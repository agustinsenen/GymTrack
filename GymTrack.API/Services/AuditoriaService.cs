using Microsoft.EntityFrameworkCore;
using GymTrack.API.Data;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuditoriaService> _logger;

        public AuditoriaService(AppDbContext context, ILogger<AuditoriaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RegistrarAsync(
            string accion,
            string entidad,
            int? entidadId,
            int? usuarioId,
            string? detalle = null)
        {
            var registro = new Auditoria
            {
                Fecha = FechaHelper.Ahora,
                UsuarioId = usuarioId,
                Accion = accion,
                Entidad = entidad,
                EntidadId = entidadId,
                Detalle = detalle != null && detalle.Length > 500
                    ? detalle[..500]
                    : detalle
            };

            try
            {
                _context.Auditorias.Add(registro);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // La auditoría nunca debe tumbar la operación principal
                _context.Entry(registro).State = EntityState.Detached;
                _logger.LogError(ex, "No se pudo registrar la auditoría {Accion} {Entidad}",
                    accion, entidad);
            }
        }
    }
}
