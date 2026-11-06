using GymTrack.API.DTOs.Alumno;

namespace GymTrack.API.Services.Interfaces
{
    // usuarioActualId + esAdmin: un profesor solo ve y modifica SUS alumnos,
    // el administrador ve todos
    public interface IAlumnoService
    {
        Task<List<AlumnoDto>> ListarAsync(int usuarioActualId, bool esAdmin, string? buscar, bool incluirInactivos);

        Task<AlumnoDto> ObtenerAsync(int id, int usuarioActualId, bool esAdmin);

        // Para el propio alumno (GET api/alumnos/yo)
        Task<AlumnoDto> ObtenerPropioAsync(int alumnoId);

        Task<AlumnoDto> CrearAsync(CrearAlumnoDto dto, int usuarioActualId, bool esAdmin);

        Task<AlumnoDto> ActualizarAsync(int id, ActualizarAlumnoDto dto, int usuarioActualId, bool esAdmin);

        Task DarDeBajaAsync(int id, int usuarioActualId, bool esAdmin);

        Task<AlumnoDto> ReasignarProfesorAsync(int id, int profesorId, int adminId);

        // Lanza excepción si el alumno no existe o no es del profesor.
        // La usan otros servicios (cuotas, rutinas, progreso).
        Task VerificarAccesoAsync(int alumnoId, int usuarioActualId, bool esAdmin);
    }
}
