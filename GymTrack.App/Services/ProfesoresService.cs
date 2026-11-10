using GymTrack.App.Models;

namespace GymTrack.App.Services;

// Gestión de profesores (solo la usa el administrador)
public class ProfesoresService
{
    private readonly ApiClient _api;

    public ProfesoresService(ApiClient api)
    {
        _api = api;
    }

    public Task<(List<ProfesorDto>? Datos, string? Error)> ListarAsync(bool incluirInactivos = false)
        => _api.GetAsync<List<ProfesorDto>>(
            $"api/profesores?incluirInactivos={(incluirInactivos ? "true" : "false")}");

    public Task<(ProfesorDto? Datos, string? Error)> ObtenerAsync(int id)
        => _api.GetAsync<ProfesorDto>($"api/profesores/{id}");

    public Task<(ProfesorDto? Datos, string? Error)> CrearAsync(GuardarProfesorDto dto)
        => _api.PostAsync<GuardarProfesorDto, ProfesorDto>("api/profesores", dto);

    public Task<(ProfesorDto? Datos, string? Error)> ActualizarAsync(int id, GuardarProfesorDto dto)
        => _api.PutAsync<GuardarProfesorDto, ProfesorDto>($"api/profesores/{id}", dto);

    public Task<string?> DarDeBajaAsync(int id)
        => _api.DeleteAsync($"api/profesores/{id}");
}
