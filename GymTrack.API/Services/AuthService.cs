using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using GymTrack.API.Data;
using GymTrack.API.DTOs.Auth;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GymTrack.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IAuditoriaService _auditoria;

        public AuthService(
            AppDbContext context,
            IConfiguration configuration,
            IAuditoriaService auditoria)
        {
            _context = context;
            _configuration = configuration;
            _auditoria = auditoria;
        }

        public async Task<LoginRespuestaDto?> LoginAsync(LoginDto dto)
        {
            var texto = dto.Usuario.Trim();

            // Se puede entrar con email o con DNI (el DNI está en la tabla Alumnos)
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Alumno)
                .FirstOrDefaultAsync(u =>
                    u.Email == texto ||
                    (u.Alumno != null && u.Alumno.Dni == texto));

            if (usuario == null)
            {
                await _auditoria.RegistrarAsync("LOGIN_FALLIDO", "Usuario", null, null,
                    $"Usuario inexistente: {texto}");
                return null;
            }

            if (!usuario.Activo)
            {
                await _auditoria.RegistrarAsync("LOGIN_FALLIDO", "Usuario", usuario.Id, null,
                    "Usuario dado de baja");
                return null;
            }

            bool passwordCorrecta;

            try
            {
                passwordCorrecta = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                passwordCorrecta = false;
            }

            if (!passwordCorrecta)
            {
                await _auditoria.RegistrarAsync("LOGIN_FALLIDO", "Usuario", usuario.Id, null,
                    "Contraseña incorrecta");
                return null;
            }

            await _auditoria.RegistrarAsync("LOGIN", "Usuario", usuario.Id, usuario.Id);

            return new LoginRespuestaDto
            {
                Token = GenerarToken(usuario),
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Rol = usuario.Rol!.Nombre,
                AlumnoId = usuario.Alumno?.Id
            };
        }

        private string GenerarToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol!.Nombre)
            };

            // Si es alumno, guardamos su AlumnoId en el token para no buscarlo en cada request
            if (usuario.Alumno != null)
            {
                claims.Add(new Claim("alumnoId", usuario.Alumno.Id.ToString()));
            }

            var clave = _configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(clave))
            {
                throw new InvalidOperationException("No se configuró la clave JWT.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave));

            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credenciales
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
