using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.DTOs.Progreso
{
    // Lo que manda el alumno al tocar "Marcar como Realizado"
    public class RegistrarProgresoDto
    {
        // Id del RutinaEjercicio (viene en RutinaItemDto.Id)
        [Range(1, int.MaxValue)]
        public int RutinaEjercicioId { get; set; }

        [Range(0, 1000, ErrorMessage = "El peso debe estar entre 0 y 1000 kg.")]
        public decimal PesoKg { get; set; }

        public bool Realizado { get; set; } = true;
    }

    public class HistorialProgresoDto
    {
        public int Id { get; set; }

        public int EjercicioId { get; set; }

        public string EjercicioNombre { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public decimal PesoKg { get; set; }

        public bool Realizado { get; set; }
    }

    // Para los chips de la pantalla "Mi progreso" ("Press de banca: 80 kg, +15 kg")
    public class ProgresoEjercicioDto
    {
        public int EjercicioId { get; set; }

        public string EjercicioNombre { get; set; } = string.Empty;

        public string GrupoMuscular { get; set; } = string.Empty;

        public decimal PrimerPesoKg { get; set; }

        public decimal UltimoPesoKg { get; set; }

        public decimal DiferenciaKg { get; set; }

        public int CantidadRegistros { get; set; }

        public DateTime UltimaFecha { get; set; }
    }

    public class AsistenciasMesDto
    {
        public int Anio { get; set; }

        public int Mes { get; set; }

        public int Total { get; set; }

        // Días marcados en el calendario
        public List<DateTime> Fechas { get; set; } = new();
    }
}
