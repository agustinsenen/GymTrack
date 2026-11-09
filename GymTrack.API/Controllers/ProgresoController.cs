using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GymTrack.API.DTOs.Progreso;
using GymTrack.API.Helpers;
using GymTrack.API.Models;
using GymTrack.API.Services.Interfaces;

namespace GymTrack.API.Controllers
{
    // Progreso y asistencias cuelgan del alumno: api/alumnos/...
    [ApiController]
    [Route("api/alumnos")]
    [Authorize]
    public class ProgresoController : ControllerBase
    {
        private const string AdminOProfesor = Rol.Administrador + "," + Rol.Profesor;

        private readonly IProgresoService _progresoService;
        private readonly IAlumnoService _alumnoService;

        public ProgresoController(IProgresoService progresoService, IAlumnoService alumnoService)
        {
            _progresoService = progresoService;
            _alumnoService = alumnoService;
        }

        // ---------- Alumno ----------

        // POST: api/alumnos/yo/progreso  (botón "Marcar como Realizado")
        [HttpPost("yo/progreso")]
        [Authorize(Roles = Rol.Alumno)]
        public async Task<ActionResult<HistorialProgresoDto>> Registrar(RegistrarProgresoDto dto)
        {
            return Ok(await _progresoService.RegistrarAsync(User.ObtenerAlumnoId(), dto));
        }

        // GET: api/alumnos/yo/progreso
        [HttpGet("yo/progreso")]
        [Authorize(Roles = Rol.Alumno)]
        public async Task<ActionResult<List<ProgresoEjercicioDto>>> ResumenPropio()
        {
            return Ok(await _progresoService.ResumenPorEjercicioAsync(User.ObtenerAlumnoId()));
        }

        // GET: api/alumnos/yo/progreso/3  (datos para el gráfico de un ejercicio)
        [HttpGet("yo/progreso/{ejercicioId:int}")]
        [Authorize(Roles = Rol.Alumno)]
        public async Task<ActionResult<List<HistorialProgresoDto>>> HistorialPropio(int ejercicioId)
        {
            return Ok(await _progresoService.HistorialAsync(User.ObtenerAlumnoId(), ejercicioId));
        }

        // GET: api/alumnos/yo/asistencias?anio=2026&mes=11
        [HttpGet("yo/asistencias")]
        [Authorize(Roles = Rol.Alumno)]
        public async Task<ActionResult<AsistenciasMesDto>> AsistenciasPropias(int? anio = null, int? mes = null)
        {
            var hoy = FechaHelper.Hoy;

            return Ok(await _progresoService.AsistenciasAsync(
                User.ObtenerAlumnoId(), anio ?? hoy.Year, mes ?? hoy.Month));
        }

        // ---------- Profesor / Administrador ----------

        // GET: api/alumnos/5/progreso
        [HttpGet("{id:int}/progreso")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<List<ProgresoEjercicioDto>>> Resumen(int id)
        {
            await _alumnoService.VerificarAccesoAsync(id, User.ObtenerUsuarioId(), User.EsAdmin());

            return Ok(await _progresoService.ResumenPorEjercicioAsync(id));
        }

        // GET: api/alumnos/5/progreso/3
        [HttpGet("{id:int}/progreso/{ejercicioId:int}")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<List<HistorialProgresoDto>>> Historial(int id, int ejercicioId)
        {
            await _alumnoService.VerificarAccesoAsync(id, User.ObtenerUsuarioId(), User.EsAdmin());

            return Ok(await _progresoService.HistorialAsync(id, ejercicioId));
        }

        // GET: api/alumnos/5/asistencias?anio=2026&mes=11
        [HttpGet("{id:int}/asistencias")]
        [Authorize(Roles = AdminOProfesor)]
        public async Task<ActionResult<AsistenciasMesDto>> Asistencias(int id, int? anio = null, int? mes = null)
        {
            await _alumnoService.VerificarAccesoAsync(id, User.ObtenerUsuarioId(), User.EsAdmin());

            var hoy = FechaHelper.Hoy;

            return Ok(await _progresoService.AsistenciasAsync(id, anio ?? hoy.Year, mes ?? hoy.Month));
        }
    }
}
