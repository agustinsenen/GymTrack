using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.DTOs.Rutina
{
    // ---------- Lo que devuelve la API ----------

    public class RutinaDto
    {
        public int Id { get; set; }

        public int AlumnoId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public string ProfesorNombre { get; set; } = string.Empty;

        // Agrupada por día: Lunes, Miércoles, Viernes...
        public List<RutinaDiaDto> Dias { get; set; } = new();
    }

    public class RutinaDiaDto
    {
        public int DiaSemana { get; set; }

        public string NombreDia { get; set; } = string.Empty;

        public List<RutinaItemDto> Ejercicios { get; set; } = new();
    }

    public class RutinaItemDto
    {
        // Id de RutinaEjercicio: es el que se manda al registrar progreso
        public int Id { get; set; }

        public int EjercicioId { get; set; }

        public string EjercicioNombre { get; set; } = string.Empty;

        public string GrupoMuscular { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string? FotoUrl { get; set; }

        public string? VideoUrl { get; set; }

        public int DiaSemana { get; set; }

        public int Series { get; set; }

        public int Repeticiones { get; set; }

        public decimal? KgObjetivo { get; set; }

        public int Orden { get; set; }

        // Último peso que registró el alumno en este ejercicio
        public decimal? UltimoPesoKg { get; set; }

        // Para pintar el círculo verde de la vista del alumno
        public bool RealizadoHoy { get; set; }
    }

    // ---------- Lo que manda el profesor ----------

    // Guardar = reemplaza la rutina activa por una nueva (la anterior queda en el historial)
    public class GuardarRutinaDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "La rutina tiene que tener al menos un ejercicio.")]
        public List<ItemRutinaDto> Ejercicios { get; set; } = new();
    }

    public class ItemRutinaDto
    {
        [Range(1, int.MaxValue)]
        public int EjercicioId { get; set; }

        [Range(1, 7, ErrorMessage = "El día debe ser de 1 (lunes) a 7 (domingo).")]
        public int DiaSemana { get; set; }

        [Range(1, 20)]
        public int Series { get; set; }

        [Range(1, 100)]
        public int Repeticiones { get; set; }

        [Range(0, 1000)]
        public decimal? KgObjetivo { get; set; }

        // Opcional: si viene 0 se usa el orden en que llegan
        public int Orden { get; set; }
    }
}
