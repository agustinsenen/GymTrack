using GymTrack.App.Models;

namespace GymTrack.App.Services;

public class RutinasService
{
    private readonly ApiClient _api;

    public RutinasService(ApiClient api)
    {
        _api = api;
    }

    // alumnoId = null -> la rutina del alumno logueado
    public Task<(RutinaDto? Datos, string? Error)> ObtenerAsync(int? alumnoId = null)
        => _api.GetAsync<RutinaDto>(
            alumnoId.HasValue ? $"api/alumnos/{alumnoId}/rutina" : "api/alumnos/yo/rutina");

    // Crea o reemplaza la rutina del alumno (profesor / administrador)
    public Task<(RutinaDto? Datos, string? Error)> GuardarAsync(int alumnoId, GuardarRutinaDto dto)
        => _api.PutAsync<GuardarRutinaDto, RutinaDto>($"api/alumnos/{alumnoId}/rutina", dto);
}
