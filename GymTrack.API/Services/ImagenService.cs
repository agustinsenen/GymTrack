using GymTrack.API.Exceptions;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Services
{
    // Misma lógica que GuardarImagenAsync del TP5, pero como servicio reutilizable
    public class ImagenService : IImagenService
    {
        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };

        private const long TamanoMaximo = 2 * 1024 * 1024; // 2 MB

        private readonly string _raizWeb;

        public ImagenService(IWebHostEnvironment environment)
        {
            // WebRootPath es null si la carpeta wwwroot no existe
            _raizWeb = environment.WebRootPath
                ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        }

        public async Task<string> GuardarAsync(IFormFile archivo, string carpeta)
        {
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!ExtensionesPermitidas.Contains(extension))
            {
                throw new ReglaDeNegocioException("La imagen debe ser JPG, PNG o WEBP.");
            }

            if (archivo.Length == 0)
            {
                throw new ReglaDeNegocioException("La imagen está vacía.");
            }

            if (archivo.Length > TamanoMaximo)
            {
                throw new ReglaDeNegocioException("La imagen no puede superar los 2 MB.");
            }

            var directorio = Path.Combine(_raizWeb, "uploads", carpeta);
            Directory.CreateDirectory(directorio);

            var nombreArchivo = $"{Guid.NewGuid()}{extension}";

            await using (var stream = new FileStream(Path.Combine(directorio, nombreArchivo), FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return $"/uploads/{carpeta}/{nombreArchivo}";
        }

        public void Eliminar(string? rutaPublica)
        {
            if (string.IsNullOrWhiteSpace(rutaPublica))
            {
                return;
            }

            var relativa = rutaPublica.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var rutaArchivo = Path.GetFullPath(Path.Combine(_raizWeb, relativa));
            var carpetaUploads = Path.GetFullPath(Path.Combine(_raizWeb, "uploads"));

            // Solo se borran archivos dentro de wwwroot/uploads
            if (rutaArchivo.StartsWith(carpetaUploads) && File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }
        }
    }
}
