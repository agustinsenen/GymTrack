using Microsoft.EntityFrameworkCore;
using GymTrack.API.Data;
using GymTrack.API.DTOs.Resumen;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Services
{
    public class ResumenService : IResumenService
    {
        private readonly AppDbContext _context;

        public ResumenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResumenDto> ObtenerAsync(int usuarioActualId, bool esAdmin)
        {
            var hoy = FechaHelper.Hoy;

            var alumnos = _context.Alumnos.Where(a => a.Usuario!.Activo);

            if (!esAdmin)
            {
                alumnos = alumnos.Where(a => a.ProfesorId == usuarioActualId);
            }

            var total = await alumnos.CountAsync();

            // Cuota al día = tiene algún pago que lo cubre hasta hoy o más adelante
            var alDia = await alumnos.CountAsync(a => a.Pagos.Any(p => p.NuevaFechaVencimiento >= hoy));

            var resumen = new ResumenDto
            {
                TotalAlumnos = total,
                CuotaAlDia = alDia,
                CuotasVencidas = total - alDia
            };

            if (esAdmin)
            {
                resumen.TotalProfesores = await _context.Usuarios
                    .CountAsync(u => u.Activo && u.Rol!.Nombre == Rol.Profesor);
            }

            return resumen;
        }
    }
}
