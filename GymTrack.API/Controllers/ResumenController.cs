using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymTrack.API.DTOs.Resumen;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Rol.Administrador + "," + Rol.Profesor)]
    public class ResumenController : ControllerBase
    {
        private readonly IResumenService _resumenService;

        public ResumenController(IResumenService resumenService)
        {
            _resumenService = resumenService;
        }

        // GET: api/resumen
        // Admin: totales del gimnasio. Profesor: solo de sus alumnos.
        [HttpGet]
        public async Task<ActionResult<ResumenDto>> Obtener()
        {
            return Ok(await _resumenService.ObtenerAsync(User.ObtenerUsuarioId(), User.EsAdmin()));
        }
    }
}
