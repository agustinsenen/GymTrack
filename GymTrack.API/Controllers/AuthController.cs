using Microsoft.AspNetCore.Mvc;
using GymTrack.API.DTOs.Auth;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        // No hay registro público: los alumnos los crea un profesor
        // y los profesores los crea el administrador.
        [HttpPost("login")]
        public async Task<ActionResult<LoginRespuestaDto>> Login(LoginDto dto)
        {
            var respuesta = await _authService.LoginAsync(dto);

            if (respuesta == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario o contraseña incorrectos."
                });
            }

            return Ok(respuesta);
        }
    }
}
