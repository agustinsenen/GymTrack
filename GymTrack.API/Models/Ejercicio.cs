using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.Models
{
    // Catálogo de ejercicios del gimnasio
    public class Ejercicio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [MaxLength(300)]
        public string? Observacion { get; set; }

        // Foto guardada en wwwroot/uploads (igual que las imágenes de productos del TP5)
        [MaxLength(300)]
        public string? FotoUrl { get; set; }

        // Video referencial por URL (YouTube, etc.)
        [MaxLength(300)]
        public string? VideoUrl { get; set; }

        public int GrupoMuscularId { get; set; }

        public GrupoMuscular? GrupoMuscular { get; set; }

        // Baja lógica: no se borra porque puede estar en rutinas o en el historial
        public bool Activo { get; set; } = true;
    }
}
