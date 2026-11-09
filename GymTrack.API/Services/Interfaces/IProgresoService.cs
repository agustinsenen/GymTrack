using GymTrack.API.DTOs.Progreso;

namespace GymTrack.API.Services.Interfaces
{
    // El permiso sobre el alumno lo verifica el controlador con IAlumnoService.VerificarAccesoAsync
    public interface IProgresoService
    {
        // Guarda el kg del día y, si es el primer ejercicio realizado hoy, crea la asistencia
        Task<HistorialProgresoDto> RegistrarAsync(int alumnoId, RegistrarProgresoDto dto);

        Task<List<ProgresoEjercicioDto>> ResumenPorEjercicioAsync(int alumnoId);

        Task<List<HistorialProgresoDto>> HistorialAsync(int alumnoId, int ejercicioId);

        Task<AsistenciasMesDto> AsistenciasAsync(int alumnoId, int anio, int mes);
    }
}
