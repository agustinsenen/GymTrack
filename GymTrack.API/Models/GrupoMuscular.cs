using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.Models
{
    public class GrupoMuscular
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Ejercicio> Ejercicios { get; set; } = new List<Ejercicio>();
    }
}
