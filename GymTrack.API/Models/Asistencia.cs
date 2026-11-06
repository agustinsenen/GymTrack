using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.Models
{
    // Se crea sola cuando el alumno marca su primer ejercicio del día.
    // Hay un índice único (AlumnoId, Fecha): una asistencia por día.
    public class Asistencia
    {
        [Key]
        public int Id { get; set; }

        public int AlumnoId { get; set; }

        public Alumno? Alumno { get; set; }

        // Solo la fecha (columna "date" en SQL Server)
        public DateTime Fecha { get; set; }
    }
}
