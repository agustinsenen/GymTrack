using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.Models
{
    // Historial de pagos de cuota (control informativo, sin cobro digital)
    public class PagoCuota
    {
        [Key]
        public int Id { get; set; }

        public int AlumnoId { get; set; }

        public Alumno? Alumno { get; set; }

        // Profesor o administrador que registró el pago
        public int RegistradoPorId { get; set; }

        public Usuario? RegistradoPor { get; set; }

        // Día en que el alumno pagó
        public DateTime FechaPago { get; set; }

        // Hasta cuándo queda cubierto con este pago
        public DateTime NuevaFechaVencimiento { get; set; }

        [MaxLength(200)]
        public string? Observacion { get; set; }
    }
}
