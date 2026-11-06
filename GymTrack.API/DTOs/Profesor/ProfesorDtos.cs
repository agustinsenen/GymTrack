using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.DTOs.Profesor
{
    public class ProfesorDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool Activo { get; set; }

        // Para las tarjetas del panel admin ("Julian Mangas - 2 alumnos")
        public int CantidadAlumnos { get; set; }
    }

    public class CrearProfesorDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }

    public class ActualizarProfesorDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        // Opcional: solo se cambia si viene cargada
        [MinLength(6)]
        public string? Password { get; set; }

        public bool Activo { get; set; } = true;
    }
}
