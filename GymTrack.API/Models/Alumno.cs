using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.Models
{
    // Extiende a Usuario (1:1) con los datos propios del alumno
    public class Alumno
    {
        [Key]
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public Usuario? Usuario { get; set; }

        // Profesor que lo registró (el administrador puede reasignarlo)
        public int ProfesorId { get; set; }

        public Usuario? Profesor { get; set; }

        [Required]
        [MaxLength(10)]
        public string Dni { get; set; } = string.Empty;

        // La fecha de vencimiento NO se guarda acá: se obtiene del último PagoCuota
        // (así no hay datos duplicados y se respeta la 3FN)

        public ICollection<Rutina> Rutinas { get; set; } = new List<Rutina>();

        public ICollection<PagoCuota> Pagos { get; set; } = new List<PagoCuota>();

        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();

        public ICollection<HistorialProgreso> Progresos { get; set; } = new List<HistorialProgreso>();
    }
}
