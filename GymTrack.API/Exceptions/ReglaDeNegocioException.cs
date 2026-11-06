namespace GymTrack.API.Exceptions
{
    // Error de validación de negocio: el middleware lo devuelve como 400 con el mensaje
    public class ReglaDeNegocioException : Exception
    {
        public ReglaDeNegocioException(string mensaje) : base(mensaje)
        {
        }
    }
}
