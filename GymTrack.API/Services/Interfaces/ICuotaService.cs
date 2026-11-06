using GymTrack.API.DTOs.Cuota;

namespace GymTrack.API.Services.Interfaces
{
    public interface ICuotaService
    {
        Task<List<PagoCuotaDto>> ListarPagosAsync(int alumnoId);

        Task<PagoCuotaDto> RegistrarPagoAsync(int alumnoId, RegistrarPagoDto dto, int registradoPorId);
    }
}
