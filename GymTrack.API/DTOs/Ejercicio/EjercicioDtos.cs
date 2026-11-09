using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.DTOs.Ejercicio
{
    public class GrupoMuscularDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }

    public class EjercicioDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string? Observacion { get; set; }

        // Ruta relativa ("/uploads/ejercicios/x.jpg"): la app le antepone la URL de la API
        public string? FotoUrl { get; set; }

        public string? VideoUrl { get; set; }

        public int GrupoMuscularId { get; set; }

        public string GrupoMuscularNombre { get; set; } = string.Empty;

        public bool Activo { get; set; }
    }

    // Se envían como multipart/form-data ([FromForm]) porque incluyen la foto
    public class CrearEjercicioDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [MaxLength(300)]
        public string? Observacion { get; set; }

        [Url(ErrorMessage = "El video debe ser una URL válida (ej.: https://youtube.com/...).")]
        [MaxLength(300)]
        public string? VideoUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Elegí un grupo muscular.")]
        public int GrupoMuscularId { get; set; }

        public IFormFile? Foto { get; set; }
    }

    public class ActualizarEjercicioDto : CrearEjercicioDto
    {
        public bool Activo { get; set; } = true;

        // true = borra la foto actual sin subir otra
        public bool QuitarFoto { get; set; }
    }
}
