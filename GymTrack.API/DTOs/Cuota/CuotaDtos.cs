using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.DTOs.Cuota
{
    public class PagoCuotaDto
    {
        public int Id { get; set; }

        public int AlumnoId { get; set; }

        public DateTime FechaPago { get; set; }

        public DateTime NuevaFechaVencimiento { get; set; }

        public string? Observacion { get; set; }

        public string RegistradoPorNombre { get; set; } = string.Empty;
    }

    public class RegistrarPagoDto
    {
        // Opcional: si no viene, se toma la fecha de hoy
        public DateTime? FechaPago { get; set; }

        // Opcional: si no viene, se suma 1 mes (ver CuotaService)
        public DateTime? NuevaFechaVencimiento { get; set; }

        [MaxLength(200)]
        public string? Observacion { get; set; }
    }
}
