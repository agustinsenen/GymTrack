using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.Models
{
    public class Rutina
    {
        [Key]
        public int Id { get; set; }

        public int AlumnoId { get; set; }

        public Alumno? Alumno { get; set; }

        // Profesor que armó la rutina
        public int ProfesorId { get; set; }

        public Usuario? Profesor { get; set; }

        public DateTime FechaCreacion { get; set; }

        // Un alumno tiene una sola rutina activa; las anteriores quedan como historial
        public bool Activa { get; set; } = true;

        public ICollection<RutinaEjercicio> Ejercicios { get; set; } = new List<RutinaEjercicio>();
    }
}
