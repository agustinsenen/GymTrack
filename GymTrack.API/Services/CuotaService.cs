using Microsoft.EntityFrameworkCore;
using GymTrack.API.Data;
using GymTrack.API.DTOs.Cuota;
using GymTrack.API.Exceptions;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Services
{
    // El permiso sobre el alumno lo verifica el controlador con IAlumnoService.VerificarAccesoAsync
    public class CuotaService : ICuotaService
    {
        private readonly AppDbContext _context;
        private readonly IAuditoriaService _auditoria;

        public CuotaService(AppDbContext context, IAuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        public async Task<List<PagoCuotaDto>> ListarPagosAsync(int alumnoId)
        {
            return await _context.PagosCuota
                .Where(p => p.AlumnoId == alumnoId)
                .OrderByDescending(p => p.FechaPago)
                .ThenByDescending(p => p.Id)
                .Select(p => new PagoCuotaDto
                {
                    Id = p.Id,
                    AlumnoId = p.AlumnoId,
                    FechaPago = p.FechaPago,
                    NuevaFechaVencimiento = p.NuevaFechaVencimiento,
                    Observacion = p.Observacion,
                    RegistradoPorNombre = p.RegistradoPor!.Nombre
                })
                .ToListAsync();
        }

        public async Task<PagoCuotaDto> RegistrarPagoAsync(int alumnoId, RegistrarPagoDto dto, int registradoPorId)
        {
            var hoy = FechaHelper.Hoy;
            var fechaPago = (dto.FechaPago ?? hoy).Date;

            if (fechaPago > hoy)
            {
                throw new ReglaDeNegocioException("La fecha de pago no puede ser futura.");
            }

            var vencimientoActual = await _context.PagosCuota
                .Where(p => p.AlumnoId == alumnoId)
                .MaxAsync(p => (DateTime?)p.NuevaFechaVencimiento);

            DateTime nuevoVencimiento;

            if (dto.NuevaFechaVencimiento.HasValue)
            {
                nuevoVencimiento = dto.NuevaFechaVencimiento.Value.Date;
            }
            else if (vencimientoActual.HasValue && vencimientoActual.Value >= fechaPago)
            {
                // Pagó antes de que venciera: se extiende 1 mes desde el vencimiento actual
                nuevoVencimiento = vencimientoActual.Value.AddMonths(1);
            }
            else
            {
                // Nunca pagó o ya estaba vencida: 1 mes desde el día de pago
                nuevoVencimiento = fechaPago.AddMonths(1);
            }

            if (nuevoVencimiento <= fechaPago)
            {
                throw new ReglaDeNegocioException("El nuevo vencimiento debe ser posterior a la fecha de pago.");
            }

            var pago = new PagoCuota
            {
                AlumnoId = alumnoId,
                RegistradoPorId = registradoPorId,
                FechaPago = fechaPago,
                NuevaFechaVencimiento = nuevoVencimiento,
                Observacion = string.IsNullOrWhiteSpace(dto.Observacion) ? null : dto.Observacion.Trim()
            };

            _context.PagosCuota.Add(pago);
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("PAGO_CUOTA", "Alumno", alumnoId, registradoPorId,
                $"Pago {fechaPago:dd/MM/yyyy}, vence {nuevoVencimiento:dd/MM/yyyy}");

            return (await ListarPagosAsync(alumnoId)).First(p => p.Id == pago.Id);
        }
    }
}
