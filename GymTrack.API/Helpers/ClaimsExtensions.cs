using GymTrack.API.Exceptions;
using GymTrack.API.Models;
using System.Security.Claims;

namespace GymTrack.API.Helpers
{
    // Atajos para leer los datos del token en los controladores: User.ObtenerUsuarioId(), etc.
    public static class ClaimsExtensions
    {
        public static int ObtenerUsuarioId(this ClaimsPrincipal user) =>
            int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        public static bool EsAdmin(this ClaimsPrincipal user) =>
            user.IsInRole(Rol.Administrador);

        public static int ObtenerAlumnoId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("alumnoId")
                ?? throw new AccesoDenegadoException("El usuario no es un alumno.");

            return int.Parse(claim.Value);
        }
    }
}
