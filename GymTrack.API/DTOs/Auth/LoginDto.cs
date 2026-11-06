using System.ComponentModel.DataAnnotations;

namespace GymTrack.API.DTOs.Auth
{
    public class LoginDto
    {
        // Email o DNI (como dice la pantalla de login del Figma)
        [Required]
        public string Usuario { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
