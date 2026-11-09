namespace GymTrack.API.Services.Interfaces
{
    public interface IImagenService
    {
        // Guarda la imagen en wwwroot/uploads/{carpeta} y devuelve la ruta pública
        // (ej.: "/uploads/ejercicios/abc.jpg")
        Task<string> GuardarAsync(IFormFile archivo, string carpeta);

        void Eliminar(string? rutaPublica);
    }
}
