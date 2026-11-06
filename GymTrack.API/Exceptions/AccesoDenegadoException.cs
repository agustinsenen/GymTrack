namespace GymTrack.API.Exceptions
{
    // El usuario está logueado pero no puede tocar ese recurso
    // (ej.: un profesor queriendo ver un alumno de otro profesor): se devuelve 403
    public class AccesoDenegadoException : Exception
    {
        public AccesoDenegadoException(string mensaje) : base(mensaje)
        {
        }
    }
}
