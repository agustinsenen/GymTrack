namespace GymTrack.API.Helpers
{
    // Fecha y hora de Argentina, sin importar el huso horario del servidor
    // (el hosting puede estar en otro país). Usar siempre esto en vez de DateTime.Now.
    public static class FechaHelper
    {
        private static readonly TimeZoneInfo ZonaArgentina = ObtenerZona();

        private static TimeZoneInfo ObtenerZona()
        {
            try
            {
                // Nombre de la zona en Windows (MonsterASP corre en Windows)
                return TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                // Nombre de la zona en Linux/Mac
                return TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Buenos_Aires");
            }
        }

        public static DateTime Ahora =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ZonaArgentina);

        public static DateTime Hoy => Ahora.Date;
    }
}
