using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymTrack.API.DTOs.Profesor;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Rol.Administrador)]
    public class ProfesoresController : ControllerBase
    {
        private readonly IProfesorService _profesorService;

        public ProfesoresController(IProfesorService profesorService)
        {
            _profesorService = profesorService;
        }

        // GET: api/profesores?incluirInactivos=false
        [HttpGet]
        public async Task<ActionResult<List<ProfesorDto>>> Listar(bool incluirInactivos = false)
        {
            return Ok(await _profesorService.ListarAsync(incluirInactivos));
        }

        // GET: api/profesores/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProfesorDto>> Obtener(int id)
        {
            return Ok(await _profesorService.ObtenerAsync(id));
        }

        // POST: api/profesores
        [HttpPost]
        public async Task<ActionResult<ProfesorDto>> Crear(CrearProfesorDto dto)
        {
            var profesor = await _profesorService.CrearAsync(dto, User.ObtenerUsuarioId());

            return CreatedAtAction(nameof(Obtener), new { id = profesor.Id }, profesor);
        }

        // PUT: api/profesores/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProfesorDto>> Actualizar(int id, ActualizarProfesorDto dto)
        {
            return Ok(await _profesorService.ActualizarAsync(id, dto, User.ObtenerUsuarioId()));
        }

        // DELETE: api/profesores/5  (baja lógica)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DarDeBaja(int id)
        {
            await _profesorService.DarDeBajaAsync(id, User.ObtenerUsuarioId());

            return NoContent();
        }
    }
}
