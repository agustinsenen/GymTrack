using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymTrack.API.DTOs.Rutina;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Controllers
{
    // Las rutinas cuelgan del alumno: api/alumnos/{id}/rutina
    [ApiController]
    [Route("api/alumnos")]
    [Authorize]
    public class RutinasController : ControllerBase
    {
        private const string AdminOProfesor = Rol.Administrador + "," + Rol.Profesor;

        private readonly IRutinaService _rutinaService;
        private readonly IAlumnoService _alumnoService;

        public RutinasController(IRutinaService rutinaService, IAlumnoService alumnoService)
        {
            _rutinaService = rutinaService;
            _alumnoService = alumnoService;
        }

        // GET: api/alumnos/5/rutina
        [HttpGet("{id:int}/rutina")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<RutinaDto>> ObtenerRutina(int id)
        {
            await _alumnoService.VerificarAccesoAsync(id, User.ObtenerUsuarioId(), User.EsAdmin());

            return Ok(await _rutinaService.ObtenerActivaAsync(id));
        }

        // PUT: api/alumnos/5/rutina  (crea o reemplaza la rutina semanal)
        [HttpPut("{id:int}/rutina")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<RutinaDto>> GuardarRutina(int id, GuardarRutinaDto dto)
        {
            await _alumnoService.VerificarAccesoAsync(id, User.ObtenerUsuarioId(), User.EsAdmin());

            return Ok(await _rutinaService.GuardarAsync(id, dto, User.ObtenerUsuarioId()));
        }

        // GET: api/alumnos/yo/rutina  (el alumno solo puede verla, nunca modificarla)
        [HttpGet("yo/rutina")]
        [Authorize(Roles = Rol.Alumno)]
        public async Task<ActionResult<RutinaDto>> ObtenerRutinaPropia()
        {
            return Ok(await _rutinaService.ObtenerActivaAsync(User.ObtenerAlumnoId()));
        }
    }
}
