using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.Models
{
    // Credenciales de todos los actores (Administrador, Profesor, Alumno)
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public int RolId { get; set; }

        public Rol? Rol { get; set; }

        // Baja lógica: un usuario inactivo no puede iniciar sesión
        public bool Activo { get; set; } = true;

        // Si el usuario es un alumno, acá están sus datos (relación 1:1)
        public Alumno? Alumno { get; set; }

        // Si el usuario es un profesor, estos son los alumnos a su cargo
        public ICollection<Alumno> AlumnosACargo { get; set; } = new List<Alumno>();
    }
}
