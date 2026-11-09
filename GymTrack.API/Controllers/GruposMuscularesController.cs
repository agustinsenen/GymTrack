using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymTrack.API.DTOs.Ejercicio;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GruposMuscularesController : ControllerBase
    {
        private readonly IEjercicioService _ejercicioService;

        public GruposMuscularesController(IEjercicioService ejercicioService)
        {
            _ejercicioService = ejercicioService;
        }

        // GET: api/gruposmusculares  (para los filtros "Todos / Pecho / Piernas...")
        [HttpGet]
        public async Task<ActionResult<List<GrupoMuscularDto>>> Listar()
        {
            return Ok(await _ejercicioService.ListarGruposAsync());
        }
    }
}
