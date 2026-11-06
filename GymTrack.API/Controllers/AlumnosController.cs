using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymTrack.API.DTOs.Alumno;
using GymTrack.API.DTOs.Cuota;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlumnosController : ControllerBase
    {
        private const string AdminOProfesor = Rol.Administrador + "," + Rol.Profesor;

        private readonly IAlumnoService _alumnoService;
        private readonly ICuotaService _cuotaService;

        public AlumnosController(IAlumnoService alumnoService, ICuotaService cuotaService)
        {
            _alumnoService = alumnoService;
            _cuotaService = cuotaService;
        }

        // ---------- Gestión (administrador y profesor) ----------

        // GET: api/alumnos?buscar=perez&incluirInactivos=false
        [HttpGet]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<List<AlumnoDto>>> Listar(string? buscar = null, bool incluirInactivos = false)
        {
            return Ok(await _alumnoService.ListarAsync(
                User.ObtenerUsuarioId(), User.EsAdmin(), buscar, incluirInactivos));
        }

        // GET: api/alumnos/5
        [HttpGet("{id:int}")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<AlumnoDto>> Obtener(int id)
        {
            return Ok(await _alumnoService.ObtenerAsync(id, User.ObtenerUsuarioId(), User.EsAdmin()));
        }

        // POST: api/alumnos
        [HttpPost]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<AlumnoDto>> Crear(CrearAlumnoDto dto)
        {
            var alumno = await _alumnoService.CrearAsync(dto, User.ObtenerUsuarioId(), User.EsAdmin());

            return CreatedAtAction(nameof(Obtener), new { id = alumno.Id }, alumno);
        }

        // PUT: api/alumnos/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<AlumnoDto>> Actualizar(int id, ActualizarAlumnoDto dto)
        {
            return Ok(await _alumnoService.ActualizarAsync(id, dto, User.ObtenerUsuarioId(), User.EsAdmin()));
        }

        // DELETE: api/alumnos/5  (baja lógica)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<IActionResult> DarDeBaja(int id)
        {
            await _alumnoService.DarDeBajaAsync(id, User.ObtenerUsuarioId(), User.EsAdmin());

            return NoContent();
        }

        // PUT: api/alumnos/5/profesor  (solo el administrador reasigna)
        [HttpPut("{id:int}/profesor")]
        [Authorize(Roles = Rol.Administrador)]
        public async Task<ActionResult<AlumnoDto>> ReasignarProfesor(int id, ReasignarProfesorDto dto)
        {
            return Ok(await _alumnoService.ReasignarProfesorAsync(id, dto.ProfesorId, User.ObtenerUsuarioId()));
        }

        // ---------- Cuotas (administrador y profesor) ----------

        // GET: api/alumnos/5/pagos
        [HttpGet("{id:int}/pagos")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<List<PagoCuotaDto>>> ListarPagos(int id)
        {
            await _alumnoService.VerificarAccesoAsync(id, User.ObtenerUsuarioId(), User.EsAdmin());

            return Ok(await _cuotaService.ListarPagosAsync(id));
        }

        // POST: api/alumnos/5/pagos
        [HttpPost("{id:int}/pagos")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<PagoCuotaDto>> RegistrarPago(int id, RegistrarPagoDto dto)
        {
            await _alumnoService.VerificarAccesoAsync(id, User.ObtenerUsuarioId(), User.EsAdmin());

            return Ok(await _cuotaService.RegistrarPagoAsync(id, dto, User.ObtenerUsuarioId()));
        }

        // ---------- Vista del propio alumno ----------

        // GET: api/alumnos/yo
        [HttpGet("yo")]
        [Authorize(Roles = Rol.Alumno)]
        public async Task<ActionResult<AlumnoDto>> ObtenerPropio()
        {
            return Ok(await _alumnoService.ObtenerPropioAsync(User.ObtenerAlumnoId()));
        }

        // GET: api/alumnos/yo/pagos
        [HttpGet("yo/pagos")]
        [Authorize(Roles = Rol.Alumno)]
        public async Task<ActionResult<List<PagoCuotaDto>>> ListarPagosPropios()
        {
            return Ok(await _cuotaService.ListarPagosAsync(User.ObtenerAlumnoId()));
        }
    }
}
