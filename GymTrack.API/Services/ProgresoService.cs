using Microsoft.EntityFrameworkCore;
using GymTrack.API.Data;
using GymTrack.API.DTOs.Progreso;
using GymTrack.API.Exceptions;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Services
{
    public class ProgresoService : IProgresoService
    {
        private readonly AppDbContext _context;
        private readonly IAuditoriaService _auditoria;

        public ProgresoService(AppDbContext context, IAuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        public async Task<HistorialProgresoDto> RegistrarAsync(int alumnoId, RegistrarProgresoDto dto)
        {
            // El ejercicio tiene que ser de la rutina ACTIVA de este alumno
            var item = await _context.RutinaEjercicios
                .Where(re => re.Id == dto.RutinaEjercicioId
                          && re.Rutina!.AlumnoId == alumnoId
                          && re.Rutina.Activa)
                .Select(re => new { re.EjercicioId, EjercicioNombre = re.Ejercicio!.Nombre })
                .FirstOrDefaultAsync()
                ?? throw new NoEncontradoException("El ejercicio no pertenece a tu rutina actual.");

            var ahora = FechaHelper.Ahora;
            var hoy = ahora.Date;
            var manana = hoy.AddDays(1);

            // Un registro por ejercicio y por día: si ya lo cargó hoy, se actualiza
            var registro = await _context.HistorialProgresos
                .FirstOrDefaultAsync(h => h.AlumnoId == alumnoId
                                       && h.EjercicioId == item.EjercicioId
                                       && h.Fecha >= hoy
                                       && h.Fecha < manana);

            if (registro == null)
            {
                registro = new HistorialProgreso
                {
                    AlumnoId = alumnoId,
                    EjercicioId = item.EjercicioId
                };

                _context.HistorialProgresos.Add(registro);
            }

            registro.Fecha = ahora;
            registro.PesoKg = dto.PesoKg;
            registro.Realizado = dto.Realizado;

            // Asistencia automática con el primer ejercicio realizado del día
            if (dto.Realizado &&
                !await _context.Asistencias.AnyAsync(a => a.AlumnoId == alumnoId && a.Fecha == hoy))
            {
                _context.Asistencias.Add(new Asistencia { AlumnoId = alumnoId, Fecha = hoy });
            }

            await _context.SaveChangesAsync();

            var usuarioId = await _context.Alumnos
                .Where(a => a.Id == alumnoId)
                .Select(a => a.UsuarioId)
                .FirstAsync();

            await _auditoria.RegistrarAsync("PROGRESO", "Ejercicio", item.EjercicioId, usuarioId,
                $"{item.EjercicioNombre}: {dto.PesoKg} kg" + (dto.Realizado ? " (realizado)" : ""));

            return new HistorialProgresoDto
            {
                Id = registro.Id,
                EjercicioId = item.EjercicioId,
                EjercicioNombre = item.EjercicioNombre,
                Fecha = registro.Fecha,
                PesoKg = registro.PesoKg,
                Realizado = registro.Realizado
            };
        }

        public async Task<List<ProgresoEjercicioDto>> ResumenPorEjercicioAsync(int alumnoId)
        {
            var registros = await _context.HistorialProgresos
                .Where(h => h.AlumnoId == alumnoId)
                .Select(h => new
                {
                    h.EjercicioId,
                    EjercicioNombre = h.Ejercicio!.Nombre,
                    GrupoMuscular = h.Ejercicio.GrupoMuscular!.Nombre,
                    h.Fecha,
                    h.PesoKg
                })
                .ToListAsync();

            return registros
                .GroupBy(r => r.EjercicioId)
                .Select(g =>
                {
                    var ordenados = g.OrderBy(r => r.Fecha).ToList();
                    var primero = ordenados.First();
                    var ultimo = ordenados.Last();

                    return new ProgresoEjercicioDto
                    {
                        EjercicioId = g.Key,
                        EjercicioNombre = primero.EjercicioNombre,
                        GrupoMuscular = primero.GrupoMuscular,
                        PrimerPesoKg = primero.PesoKg,
                        UltimoPesoKg = ultimo.PesoKg,
                        DiferenciaKg = ultimo.PesoKg - primero.PesoKg,
                        CantidadRegistros = ordenados.Count,
                        UltimaFecha = ultimo.Fecha
                    };
                })
                .OrderByDescending(p => p.UltimaFecha)
                .ToList();
        }

        public async Task<List<HistorialProgresoDto>> HistorialAsync(int alumnoId, int ejercicioId)
        {
            return await _context.HistorialProgresos
                .Where(h => h.AlumnoId == alumnoId && h.EjercicioId == ejercicioId)
                .OrderBy(h => h.Fecha)
                .Select(h => new HistorialProgresoDto
                {
                    Id = h.Id,
                    EjercicioId = h.EjercicioId,
                    EjercicioNombre = h.Ejercicio!.Nombre,
                    Fecha = h.Fecha,
                    PesoKg = h.PesoKg,
                    Realizado = h.Realizado
                })
                .ToListAsync();
        }

        public async Task<AsistenciasMesDto> AsistenciasAsync(int alumnoId, int anio, int mes)
        {
            if (mes < 1 || mes > 12 || anio < 2000 || anio > 2100)
            {
                throw new ReglaDeNegocioException("Mes o año inválido.");
            }

            var desde = new DateTime(anio, mes, 1);
            var hasta = desde.AddMonths(1);

            var fechas = await _context.Asistencias
                .Where(a => a.AlumnoId == alumnoId && a.Fecha >= desde && a.Fecha < hasta)
                .OrderBy(a => a.Fecha)
                .Select(a => a.Fecha)
                .ToListAsync();

            return new AsistenciasMesDto
            {
                Anio = anio,
                Mes = mes,
                Total = fechas.Count,
                Fechas = fechas
            };
        }
    }
}
