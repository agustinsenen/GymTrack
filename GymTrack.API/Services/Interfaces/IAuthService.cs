using GymTrack.API.DTOs.Auth;

namespace GymTrack.API.Services.Interfaces
{
    public interface IAuthService
    {
        // Devuelve null si las credenciales son incorrectas
        Task<LoginRespuestaDto?> LoginAsync(LoginDto dto);
    }
}
