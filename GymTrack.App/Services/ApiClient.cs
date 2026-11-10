using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace GymTrack.App.Services;

// Único punto por donde la app le habla a la API.
// Cada método devuelve (Datos, Error): si Error no es null, hubo un problema.
public class ApiClient
{
    private readonly HttpClient _http;
    private readonly AuthService _auth;

    public ApiClient(HttpClient http, AuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    public Task<(T? Datos, string? Error)> GetAsync<T>(string url)
        => EnviarAsync<T>(() => _http.GetAsync(url));

    public Task<(TRespuesta? Datos, string? Error)> PostAsync<TCuerpo, TRespuesta>(
        string url, TCuerpo cuerpo)
        => EnviarAsync<TRespuesta>(() => _http.PostAsJsonAsync(url, cuerpo));

    public Task<(TRespuesta? Datos, string? Error)> PutAsync<TCuerpo, TRespuesta>(
        string url, TCuerpo cuerpo)
        => EnviarAsync<TRespuesta>(() => _http.PutAsJsonAsync(url, cuerpo));

    // Formularios con archivos (multipart/form-data)
    public Task<(TRespuesta? Datos, string? Error)> PostFormAsync<TRespuesta>(
        string url, MultipartFormDataContent contenido)
        => EnviarAsync<TRespuesta>(() => _http.PostAsync(url, contenido));

    public Task<(TRespuesta? Datos, string? Error)> PutFormAsync<TRespuesta>(
        string url, MultipartFormDataContent contenido)
        => EnviarAsync<TRespuesta>(() => _http.PutAsync(url, contenido));

    // Devuelve null si salió bien, o el mensaje de error
    public async Task<string?> DeleteAsync(string url)
    {
        try
        {
            var respuesta = await _http.DeleteAsync(url);

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            {
                _auth.Logout();
                return "Tu sesión expiró. Iniciá sesión nuevamente.";
            }

            return respuesta.IsSuccessStatusCode ? null : await LeerErrorAsync(respuesta);
        }
        catch (Exception ex)
        {
            return $"No se pudo conectar con el servidor. Revisá tu conexión. ({ex.GetType().Name}: {ex.Message})";
        }
    }

    private async Task<(T? Datos, string? Error)> EnviarAsync<T>(
        Func<Task<HttpResponseMessage>> envio)
    {
        try
        {
            var respuesta = await envio();

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Token vencido o inválido: se cierra la sesión y las
                // pantallas protegidas redirigen solas al login
                _auth.Logout();
                return (default, "Tu sesión expiró. Iniciá sesión nuevamente.");
            }

            if (!respuesta.IsSuccessStatusCode)
            {
                return (default, await LeerErrorAsync(respuesta));
            }

            var datos = await respuesta.Content.ReadFromJsonAsync<T>();

            return (datos, null);
        }
        catch (Exception ex)
        {
            return (default, $"No se pudo conectar con el servidor. Revisá tu conexión. ({ex.GetType().Name}: {ex.Message})");
        }
    }

    // La API responde { "mensaje": "..." } en los errores de negocio,
    // y { "errors": { campo: ["..."] } } en los errores de validación
    public static async Task<string> LeerErrorAsync(HttpResponseMessage respuesta)
    {
        try
        {
            var texto = await respuesta.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                using var documento = JsonDocument.Parse(texto);
                var raiz = documento.RootElement;

                if (raiz.ValueKind == JsonValueKind.Object)
                {
                    if (raiz.TryGetProperty("mensaje", out var mensaje) &&
                        mensaje.ValueKind == JsonValueKind.String)
                    {
                        return mensaje.GetString() ?? "Ocurrió un error.";
                    }

                    if (raiz.TryGetProperty("errors", out var errores) &&
                        errores.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var campo in errores.EnumerateObject())
                        {
                            foreach (var detalle in campo.Value.EnumerateArray())
                            {
                                return detalle.GetString() ?? "Hay datos inválidos.";
                            }
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            // La respuesta no era JSON: se usa el mensaje genérico de abajo
        }

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
        {
            return "No tenés permiso para hacer esto.";
        }

        return $"Error {(int)respuesta.StatusCode} del servidor.";
    }
}
