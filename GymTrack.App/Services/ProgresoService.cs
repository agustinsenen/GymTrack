using GymTrack.App.Models;

namespace GymTrack.App.Services;

// Progreso de kg y asistencias.
// alumnoId = null -> datos del alumno logueado; con id -> los ve el profesor o el administrador
public class ProgresoService
{
    private readonly ApiClient _api;

    public ProgresoService(ApiClient api)
    {
        _api = api;
    }

    private static string Base(int? alumnoId)
        => alumnoId.HasValue ? $"api/alumnos/{alumnoId}" : "api/alumnos/yo";

    // Botón "Marcar como realizado" (solo el alumno)
    public Task<(HistorialProgresoDto? Datos, string? Error)> RegistrarAsync(RegistrarProgresoDto dto)
        => _api.PostAsync<RegistrarProgresoDto, HistorialProgresoDto>("api/alumnos/yo/progreso", dto);

    public Task<(List<ProgresoEjercicioDto>? Datos, string? Error)> ResumenAsync(int? alumnoId = null)
        => _api.GetAsync<List<ProgresoEjercicioDto>>($"{Base(alumnoId)}/progreso");

    public Task<(List<HistorialProgresoDto>? Datos, string? Error)> HistorialAsync(int ejercicioId, int? alumnoId = null)
        => _api.GetAsync<List<HistorialProgresoDto>>($"{Base(alumnoId)}/progreso/{ejercicioId}");

    public Task<(AsistenciasMesDto? Datos, string? Error)> AsistenciasAsync(int anio, int mes, int? alumnoId = null)
        => _api.GetAsync<AsistenciasMesDto>($"{Base(alumnoId)}/asistencias?anio={anio}&mes={mes}");
}
