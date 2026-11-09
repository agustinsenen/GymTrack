using GymTrack.API.DTOs.Rutina;

namespace GymTrack.API.Services.Interfaces
{
    // El permiso sobre el alumno lo verifica el controlador con IAlumnoService.VerificarAccesoAsync
    public interface IRutinaService
    {
        Task<RutinaDto> ObtenerActivaAsync(int alumnoId);

        Task<RutinaDto> GuardarAsync(int alumnoId, GuardarRutinaDto dto, int usuarioId);
    }
}
