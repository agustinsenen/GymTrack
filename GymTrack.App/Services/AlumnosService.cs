using GymTrack.App.Models;

namespace GymTrack.App.Services;

// Alumnos, pagos de cuota y los números del panel
public class AlumnosService
{
    private readonly ApiClient _api;

    public AlumnosService(ApiClient api)
    {
        _api = api;
    }

    public Task<(ResumenDto? Datos, string? Error)> ResumenAsync()
        => _api.GetAsync<ResumenDto>("api/resumen");

    public Task<(List<AlumnoDto>? Datos, string? Error)> ListarAsync(string? buscar = null)
    {
        var url = "api/alumnos";

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            url += $"?buscar={Uri.EscapeDataString(buscar.Trim())}";
        }

        return _api.GetAsync<List<AlumnoDto>>(url);
    }

    public Task<(AlumnoDto? Datos, string? Error)> ObtenerAsync(int id)
        => _api.GetAsync<AlumnoDto>($"api/alumnos/{id}");

    // Los datos del alumno que está logueado
    public Task<(AlumnoDto? Datos, string? Error)> ObtenerPropioAsync()
        => _api.GetAsync<AlumnoDto>("api/alumnos/yo");

    public Task<(AlumnoDto? Datos, string? Error)> CrearAsync(GuardarAlumnoDto dto)
        => _api.PostAsync<GuardarAlumnoDto, AlumnoDto>("api/alumnos", dto);

    public Task<(AlumnoDto? Datos, string? Error)> ActualizarAsync(int id, GuardarAlumnoDto dto)
        => _api.PutAsync<GuardarAlumnoDto, AlumnoDto>($"api/alumnos/{id}", dto);

    public Task<string?> DarDeBajaAsync(int id)
        => _api.DeleteAsync($"api/alumnos/{id}");

    public Task<(AlumnoDto? Datos, string? Error)> ReasignarProfesorAsync(int id, int profesorId)
        => _api.PutAsync<ReasignarProfesorDto, AlumnoDto>(
            $"api/alumnos/{id}/profesor",
            new ReasignarProfesorDto { ProfesorId = profesorId });

    // alumnoId = null -> pagos del alumno logueado
    public Task<(List<PagoCuotaDto>? Datos, string? Error)> PagosAsync(int? alumnoId = null)
        => _api.GetAsync<List<PagoCuotaDto>>(
            alumnoId.HasValue ? $"api/alumnos/{alumnoId}/pagos" : "api/alumnos/yo/pagos");

    public Task<(PagoCuotaDto? Datos, string? Error)> RegistrarPagoAsync(int alumnoId, RegistrarPagoDto dto)
        => _api.PostAsync<RegistrarPagoDto, PagoCuotaDto>($"api/alumnos/{alumnoId}/pagos", dto);
}
