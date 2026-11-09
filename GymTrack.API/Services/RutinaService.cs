using Microsoft.EntityFrameworkCore;
using GymTrack.API.Data;
using GymTrack.API.DTOs.Rutina;
using GymTrack.API.Exceptions;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Services
{
    public class RutinaService : IRutinaService
    {
        private static readonly string[] NombresDias =
            { "", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

        private readonly AppDbContext _context;
        private readonly IAuditoriaService _auditoria;

        public RutinaService(AppDbContext context, IAuditoriaService auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        public async Task<RutinaDto> ObtenerActivaAsync(int alumnoId)
        {
            var rutina = await _context.Rutinas
                .Where(r => r.AlumnoId == alumnoId && r.Activa)
                .OrderByDescending(r => r.FechaCreacion)
                .Select(r => new
                {
                    r.Id,
                    r.AlumnoId,
                    r.FechaCreacion,
                    ProfesorNombre = r.Profesor!.Nombre,
                    Items = r.Ejercicios.Select(re => new RutinaItemDto
                    {
                        Id = re.Id,
                        EjercicioId = re.EjercicioId,
                        EjercicioNombre = re.Ejercicio!.Nombre,
                        GrupoMuscular = re.Ejercicio.GrupoMuscular!.Nombre,
                        Descripcion = re.Ejercicio.Descripcion,
                        FotoUrl = re.Ejercicio.FotoUrl,
                        VideoUrl = re.Ejercicio.VideoUrl,
                        DiaSemana = re.DiaSemana,
                        Series = re.Series,
                        Repeticiones = re.Repeticiones,
                        KgObjetivo = re.KgObjetivo,
                        Orden = re.Orden
                    }).ToList()
                })
                .FirstOrDefaultAsync()
                ?? throw new NoEncontradoException("El alumno no tiene una rutina asignada.");

            // Último peso y "realizado hoy" de cada ejercicio
            var ejercicioIds = rutina.Items.Select(i => i.EjercicioId).Distinct().ToList();
            var hoy = FechaHelper.Hoy;
            var manana = hoy.AddDays(1);

            var registros = await _context.HistorialProgresos
                .Where(h => h.AlumnoId == alumnoId && ejercicioIds.Contains(h.EjercicioId))
                .Select(h => new { h.EjercicioId, h.Fecha, h.PesoKg, h.Realizado })
                .ToListAsync();

            foreach (var item in rutina.Items)
            {
                var delEjercicio = registros.Where(r => r.EjercicioId == item.EjercicioId).ToList();

                item.UltimoPesoKg = delEjercicio
                    .OrderByDescending(r => r.Fecha)
                    .Select(r => (decimal?)r.PesoKg)
                    .FirstOrDefault();

                item.RealizadoHoy = delEjercicio.Any(r => r.Realizado && r.Fecha >= hoy && r.Fecha < manana);
            }

            return new RutinaDto
            {
                Id = rutina.Id,
                AlumnoId = rutina.AlumnoId,
                FechaCreacion = rutina.FechaCreacion,
                ProfesorNombre = rutina.ProfesorNombre,
                Dias = rutina.Items
                    .GroupBy(i => i.DiaSemana)
                    .OrderBy(g => g.Key)
                    .Select(g => new RutinaDiaDto
                    {
                        DiaSemana = g.Key,
                        NombreDia = NombresDias[g.Key],
                        Ejercicios = g.OrderBy(i => i.Orden).ToList()
                    })
                    .ToList()
            };
        }

        public async Task<RutinaDto> GuardarAsync(int alumnoId, GuardarRutinaDto dto, int usuarioId)
        {
            var ids = dto.Ejercicios.Select(e => e.EjercicioId).Distinct().ToList();

            var activos = await _context.Ejercicios
                .CountAsync(e => ids.Contains(e.Id) && e.Activo);

            if (activos != ids.Count)
            {
                throw new ReglaDeNegocioException("Uno o más ejercicios no existen o están dados de baja.");
            }

            // La rutina anterior se desactiva (no se borra): queda como historial
            var anteriores = await _context.Rutinas
                .Where(r => r.AlumnoId == alumnoId && r.Activa)
                .ToListAsync();

            foreach (var anterior in anteriores)
            {
                anterior.Activa = false;
            }

            var rutina = new Rutina
            {
                AlumnoId = alumnoId,
                ProfesorId = usuarioId,
                FechaCreacion = FechaHelper.Ahora,
                Activa = true
            };

            // Orden dentro de cada día: el que mandan o, si viene 0, el orden de llegada
            foreach (var dia in dto.Ejercicios.GroupBy(e => e.DiaSemana))
            {
                var posicion = 1;

                foreach (var item in dia)
                {
                    rutina.Ejercicios.Add(new RutinaEjercicio
                    {
                        EjercicioId = item.EjercicioId,
                        DiaSemana = item.DiaSemana,
                        Series = item.Series,
                        Repeticiones = item.Repeticiones,
                        KgObjetivo = item.KgObjetivo,
                        Orden = item.Orden > 0 ? item.Orden : posicion
                    });

                    posicion++;
                }
            }

            _context.Rutinas.Add(rutina);

            // Un solo SaveChanges: desactivar la anterior y crear la nueva van juntos
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("GUARDAR_RUTINA", "Alumno", alumnoId, usuarioId,
                $"Rutina {rutina.Id} con {rutina.Ejercicios.Count} ejercicio(s)");

            return await ObtenerActivaAsync(alumnoId);
        }
    }
}
