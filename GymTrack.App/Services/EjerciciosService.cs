using System.Net.Http.Headers;
using GymTrack.App.Models;

namespace GymTrack.App.Services;

// Catálogo de ejercicios y grupos musculares
public class EjerciciosService
{
    private readonly ApiClient _api;
    private readonly HttpClient _http;

    public EjerciciosService(ApiClient api, HttpClient http)
    {
        _api = api;
        _http = http;
    }

    public Task<(List<GrupoMuscularDto>? Datos, string? Error)> GruposAsync()
        => _api.GetAsync<List<GrupoMuscularDto>>("api/gruposmusculares");

    public Task<(List<EjercicioDto>? Datos, string? Error)> ListarAsync(int? grupoMuscularId = null)
        => _api.GetAsync<List<EjercicioDto>>(
            grupoMuscularId.HasValue
                ? $"api/ejercicios?grupoMuscularId={grupoMuscularId}"
                : "api/ejercicios");

    public Task<(EjercicioDto? Datos, string? Error)> ObtenerAsync(int id)
        => _api.GetAsync<EjercicioDto>($"api/ejercicios/{id}");

    // id = null -> crea; con id -> actualiza. La foto (JPG) es opcional.
    public Task<(EjercicioDto? Datos, string? Error)> GuardarAsync(
        int? id,
        string nombre,
        int grupoMuscularId,
        string? descripcion,
        string? observacion,
        string? videoUrl,
        byte[]? foto,
        bool quitarFoto)
    {
        var contenido = new MultipartFormDataContent
        {
            { new StringContent(nombre.Trim()), "Nombre" },
            { new StringContent(grupoMuscularId.ToString()), "GrupoMuscularId" }
        };

        if (!string.IsNullOrWhiteSpace(descripcion))
        {
            contenido.Add(new StringContent(descripcion.Trim()), "Descripcion");
        }

        if (!string.IsNullOrWhiteSpace(observacion))
        {
            contenido.Add(new StringContent(observacion.Trim()), "Observacion");
        }

        if (!string.IsNullOrWhiteSpace(videoUrl))
        {
            contenido.Add(new StringContent(videoUrl.Trim()), "VideoUrl");
        }

        if (foto != null)
        {
            var archivo = new ByteArrayContent(foto);
            archivo.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            contenido.Add(archivo, "Foto", "foto.jpg");
        }

        if (id.HasValue)
        {
            contenido.Add(new StringContent("true"), "Activo");
            contenido.Add(new StringContent(quitarFoto ? "true" : "false"), "QuitarFoto");

            return _api.PutFormAsync<EjercicioDto>($"api/ejercicios/{id}", contenido);
        }

        return _api.PostFormAsync<EjercicioDto>("api/ejercicios", contenido);
    }

    public Task<string?> DarDeBajaAsync(int id)
        => _api.DeleteAsync($"api/ejercicios/{id}");

    // La API devuelve rutas relativas (/uploads/ejercicios/xxx.jpg)
    public string? UrlImagen(string? ruta)
    {
        if (string.IsNullOrWhiteSpace(ruta))
        {
            return null;
        }

        return new Uri(_http.BaseAddress!, ruta).ToString();
    }
}
