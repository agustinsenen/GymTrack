using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymTrack.API.DTOs.Ejercicio;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EjerciciosController : ControllerBase
    {
        private const string AdminOProfesor = Rol.Administrador + "," + Rol.Profesor;

        private readonly IEjercicioService _ejercicioService;

        public EjerciciosController(IEjercicioService ejercicioService)
        {
            _ejercicioService = ejercicioService;
        }

        // GET: api/ejercicios?grupoMuscularId=1&buscar=press
        [HttpGet]
        public async Task<ActionResult<List<EjercicioDto>>> Listar(
            int? grupoMuscularId = null,
            string? buscar = null,
            bool incluirInactivos = false)
        {
            // Los alumnos nunca ven ejercicios dados de baja
            var verInactivos = incluirInactivos && !User.IsInRole(Rol.Alumno);

            return Ok(await _ejercicioService.ListarAsync(grupoMuscularId, buscar, verInactivos));
        }

        // GET: api/ejercicios/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EjercicioDto>> Obtener(int id)
        {
            return Ok(await _ejercicioService.ObtenerAsync(id));
        }

        // POST: api/ejercicios  (multipart/form-data, con foto opcional)
        [HttpPost]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<EjercicioDto>> Crear([FromForm] CrearEjercicioDto dto)
        {
            var ejercicio = await _ejercicioService.CrearAsync(dto, User.ObtenerUsuarioId());

            return CreatedAtAction(nameof(Obtener), new { id = ejercicio.Id }, ejercicio);
        }

        // PUT: api/ejercicios/5  (multipart/form-data)
        [HttpPut("{id:int}")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<EjercicioDto>> Actualizar(int id, [FromForm] ActualizarEjercicioDto dto)
        {
            return Ok(await _ejercicioService.ActualizarAsync(id, dto, User.ObtenerUsuarioId()));
        }

        // DELETE: api/ejercicios/5  (baja lógica)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<IActionResult> DarDeBaja(int id)
        {
            await _ejercicioService.DarDeBajaAsync(id, User.ObtenerUsuarioId());

            return NoContent();
        }
    }
}
