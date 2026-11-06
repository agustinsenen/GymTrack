namespace GymTrack.API.DTOs.Resumen
{
    // Los numeritos de arriba de los paneles de admin y profesor
    public class ResumenDto
    {
        public int TotalAlumnos { get; set; }

        public int CuotaAlDia { get; set; }

        public int CuotasVencidas { get; set; }

        // Solo se carga para el administrador
        public int? TotalProfesores { get; set; }
    }
}
