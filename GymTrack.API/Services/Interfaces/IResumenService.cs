using GymTrack.API.DTOs.Resumen;

namespace GymTrack.API.Services.Interfaces
{
    public interface IResumenService
    {
        Task<ResumenDto> ObtenerAsync(int usuarioActualId, bool esAdmin);
    }
}
