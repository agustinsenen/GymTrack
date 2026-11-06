using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.DTOs.Alumno
{
    public class AlumnoDto
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Dni { get; set; } = string.Empty;

        public int ProfesorId { get; set; }

        public string ProfesorNombre { get; set; } = string.Empty;

        public bool Activo { get; set; }

        // Sale del último pago registrado (null = nunca pagó)
        public DateTime? FechaVencimientoCuota { get; set; }

        // Positivo = faltan X días; negativo = vencida hace X días
        public int? DiasParaVencimiento { get; set; }

        public bool CuotaVencida { get; set; }
    }

    public class CrearAlumnoDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{7,10}$", ErrorMessage = "El DNI debe tener entre 7 y 10 números.")]
        public string Dni { get; set; } = string.Empty;

        // Opcional: si no se carga, la contraseña inicial es el DNI
        [MinLength(6)]
        public string? Password { get; set; }

        // Solo lo usa el administrador; si lo crea un profesor, queda a su cargo
        public int? ProfesorId { get; set; }
    }

    public class ActualizarAlumnoDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{7,10}$", ErrorMessage = "El DNI debe tener entre 7 y 10 números.")]
        public string Dni { get; set; } = string.Empty;

        // Opcional: solo se cambia si viene cargada
        [MinLength(6)]
        public string? Password { get; set; }

        public bool Activo { get; set; } = true;
    }

    public class ReasignarProfesorDto
    {
        [Required]
        public int ProfesorId { get; set; }
    }
}
