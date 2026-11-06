using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.Models
{
    // Cada carga (kg) que registra el alumno. Se vincula a Alumno + Ejercicio
    // (no a RutinaEjercicio) para que el historial no se pierda si la rutina cambia.
    public class HistorialProgreso
    {
        [Key]
        public int Id { get; set; }

        public int AlumnoId { get; set; }

        public Alumno? Alumno { get; set; }

        public int EjercicioId { get; set; }

        public Ejercicio? Ejercicio { get; set; }

        public DateTime Fecha { get; set; }

        public decimal PesoKg { get; set; }

        public bool Realizado { get; set; }
    }
}
