using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.Models
{
    // Tabla intermedia Rutina <-> Ejercicio con los datos de cada día
    public class RutinaEjercicio
    {
        [Key]
        public int Id { get; set; }

        public int RutinaId { get; set; }

        public Rutina? Rutina { get; set; }

        public int EjercicioId { get; set; }

        public Ejercicio? Ejercicio { get; set; }

        // 1 = Lunes ... 7 = Domingo (número para poder ordenar)
        [Range(1, 7)]
        public int DiaSemana { get; set; }

        [Range(1, 20)]
        public int Series { get; set; }

        [Range(1, 100)]
        public int Repeticiones { get; set; }

        // "Objetivo Kg" que se ve en la ficha del ejercicio (opcional)
        public decimal? KgObjetivo { get; set; }

        // Orden del ejercicio dentro del día
        public int Orden { get; set; }
    }
}
