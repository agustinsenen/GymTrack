using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.Models
{
    public class Rol
    {
        // Nombres fijos de los roles: se usan en [Authorize(Roles = ...)]
        public const string Administrador = "Administrador";
        public const string Profesor = "Profesor";
        public const string Alumno = "Alumno";

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
