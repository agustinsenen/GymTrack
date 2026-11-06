namespace GymTrack.API.DTOs.Auth
{
    public class LoginRespuestaDto
    {
        public string Token { get; set; } = string.Empty;

        public int UsuarioId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        // Solo viene cargado si el usuario es un alumno
        public int? AlumnoId { get; set; }
    }
}
