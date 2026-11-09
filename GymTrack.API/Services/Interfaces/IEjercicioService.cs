using GymTrack.API.DTOs.Ejercicio;

namespace GymTrack.API.Services.Interfaces
{
    public interface IEjercicioService
    {
        Task<List<GrupoMuscularDto>> ListarGruposAsync();

        Task<List<EjercicioDto>> ListarAsync(int? grupoMuscularId, string? buscar, bool incluirInactivos);

        Task<EjercicioDto> ObtenerAsync(int id);

        Task<EjercicioDto> CrearAsync(CrearEjercicioDto dto, int usuarioId);

        Task<EjercicioDto> ActualizarAsync(int id, ActualizarEjercicioDto dto, int usuarioId);

        Task DarDeBajaAsync(int id, int usuarioId);
    }
}
