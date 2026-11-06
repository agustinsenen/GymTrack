using GymTrack.API.DTOs.Profesor;

namespace GymTrack.API.Services.Interfaces
{
    public interface IProfesorService
    {
        Task<List<ProfesorDto>> ListarAsync(bool incluirInactivos);

        Task<ProfesorDto> ObtenerAsync(int id);

        Task<ProfesorDto> CrearAsync(CrearProfesorDto dto, int adminId);

        Task<ProfesorDto> ActualizarAsync(int id, ActualizarProfesorDto dto, int adminId);

        Task DarDeBajaAsync(int id, int adminId);
    }
}
