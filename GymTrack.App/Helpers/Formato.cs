using System.Globalization;
using GymTrack.App.Models;

namespace GymTrack.App.Helpers;

// Textos que se repiten en varias pantallas
public static class Formato
{
    public static readonly CultureInfo Cultura = new("es-AR");

    public static readonly string[] Dias =
        { "", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

    public static string Kg(decimal kg) =>
        kg.ToString("0.##", Cultura) + " kg";

    public static string Fecha(DateTime fecha) =>
        fecha.ToString("dd/MM/yyyy");

    // Primera letra del nombre, para los círculos de las listas
    public static string Inicial(string? nombre) =>
        string.IsNullOrWhiteSpace(nombre) ? "?" : nombre.Trim()[..1].ToUpper();

    public static string Cuota(AlumnoDto alumno)
    {
        var dias = alumno.DiasParaVencimiento;

        if (dias == null)
        {
            return "Sin pagos registrados";
        }

        if (dias < 0)
        {
            return $"Cuota vencida hace {-dias} día(s)";
        }

        if (dias == 0)
        {
            return "La cuota vence hoy";
        }

        return $"La cuota vence en {dias} día(s)";
    }

    // 1 = lunes ... 7 = domingo (igual que en la API)
    public static int DiaDeHoy()
    {
        var dia = (int)DateTime.Today.DayOfWeek;

        return dia == 0 ? 7 : dia;
    }
}
