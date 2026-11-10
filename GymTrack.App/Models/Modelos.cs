namespace GymTrack.App.Models;

// Copias de los DTOs de la API (la app mantiene las suyas)

// ---------- Sesión ----------

public class LoginDto
{
    public string Usuario { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginRespuestaDto
{
    public string Token { get; set; } = "";
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = "";
    public string Rol { get; set; } = "";
    public int? AlumnoId { get; set; }
}

// ---------- Paneles ----------

public class ResumenDto
{
    public int TotalAlumnos { get; set; }
    public int CuotaAlDia { get; set; }
    public int CuotasVencidas { get; set; }
    public int? TotalProfesores { get; set; }
}

// ---------- Profesores ----------

public class ProfesorDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public bool Activo { get; set; }
    public int CantidadAlumnos { get; set; }
}

public class GuardarProfesorDto
{
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Password { get; set; }
    public bool Activo { get; set; } = true;
}

// ---------- Alumnos y cuotas ----------

public class AlumnoDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string Dni { get; set; } = "";
    public int ProfesorId { get; set; }
    public string ProfesorNombre { get; set; } = "";
    public bool Activo { get; set; }
    public DateTime? FechaVencimientoCuota { get; set; }
    public int? DiasParaVencimiento { get; set; }
    public bool CuotaVencida { get; set; }
}

public class GuardarAlumnoDto
{
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string Dni { get; set; } = "";
    public string? Password { get; set; }
    public int? ProfesorId { get; set; }
    public bool Activo { get; set; } = true;
}

public class ReasignarProfesorDto
{
    public int ProfesorId { get; set; }
}

public class PagoCuotaDto
{
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    public DateTime FechaPago { get; set; }
    public DateTime NuevaFechaVencimiento { get; set; }
    public string? Observacion { get; set; }
    public string RegistradoPorNombre { get; set; } = "";
}

public class RegistrarPagoDto
{
    public DateTime? FechaPago { get; set; }
    public DateTime? NuevaFechaVencimiento { get; set; }
    public string? Observacion { get; set; }
}

// ---------- Ejercicios ----------

public class GrupoMuscularDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
}

public class EjercicioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public string? Observacion { get; set; }
    public string? FotoUrl { get; set; }
    public string? VideoUrl { get; set; }
    public int GrupoMuscularId { get; set; }
    public string GrupoMuscularNombre { get; set; } = "";
    public bool Activo { get; set; }
}

// ---------- Rutinas ----------

public class RutinaDto
{
    public int Id { get; set; }
    public int AlumnoId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string ProfesorNombre { get; set; } = "";
    public List<RutinaDiaDto> Dias { get; set; } = new();
}

public class RutinaDiaDto
{
    public int DiaSemana { get; set; }
    public string NombreDia { get; set; } = "";
    public List<RutinaItemDto> Ejercicios { get; set; } = new();
}

public class RutinaItemDto
{
    public int Id { get; set; }
    public int EjercicioId { get; set; }
    public string EjercicioNombre { get; set; } = "";
    public string GrupoMuscular { get; set; } = "";
    public string? Descripcion { get; set; }
    public string? FotoUrl { get; set; }
    public string? VideoUrl { get; set; }
    public int DiaSemana { get; set; }
    public int Series { get; set; }
    public int Repeticiones { get; set; }
    public decimal? KgObjetivo { get; set; }
    public int Orden { get; set; }
    public decimal? UltimoPesoKg { get; set; }
    public bool RealizadoHoy { get; set; }
}

public class GuardarRutinaDto
{
    public List<ItemRutinaDto> Ejercicios { get; set; } = new();
}

public class ItemRutinaDto
{
    public int EjercicioId { get; set; }
    public int DiaSemana { get; set; }
    public int Series { get; set; }
    public int Repeticiones { get; set; }
    public decimal? KgObjetivo { get; set; }
    public int Orden { get; set; }
}

// ---------- Progreso y asistencias ----------

public class RegistrarProgresoDto
{
    public int RutinaEjercicioId { get; set; }
    public decimal PesoKg { get; set; }
    public bool Realizado { get; set; } = true;
}

public class HistorialProgresoDto
{
    public int Id { get; set; }
    public int EjercicioId { get; set; }
    public string EjercicioNombre { get; set; } = "";
    public DateTime Fecha { get; set; }
    public decimal PesoKg { get; set; }
    public bool Realizado { get; set; }
}

public class ProgresoEjercicioDto
{
    public int EjercicioId { get; set; }
    public string EjercicioNombre { get; set; } = "";
    public string GrupoMuscular { get; set; } = "";
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
    public List<DateTime> Fechas { get; set; } = new();
}
