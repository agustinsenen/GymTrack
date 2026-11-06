namespace GymTrack.API.Exceptions
{
    // El recurso pedido no existe: el middleware lo devuelve como 404
    public class NoEncontradoException : Exception
    {
        public NoEncontradoException(string mensaje) : base(mensaje)
        {
        }
    }
}
