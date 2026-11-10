using GymTrack.API.Models;

namespace GymTrack.API.Data
{
    // Catálogo base de ejercicios. Se ejecuta al iniciar la API y agrega
    // solamente los ejercicios que todavía no existen (compara por nombre),
    // así que no duplica nada ni pisa lo que cargaron los profesores.
    public static class DatosIniciales
    {
        private static readonly (string Grupo, string Nombre, string Descripcion)[] Catalogo =
        {
            // Pecho
            ("Pecho", "Press banca", "Acostado en el banco, bajar la barra al pecho y empujar hasta extender los brazos."),
            ("Pecho", "Press inclinado con mancuernas", "En banco inclinado, empujar las mancuernas hacia arriba. Trabaja la parte superior del pecho."),
            ("Pecho", "Aperturas con mancuernas", "Acostado, abrir los brazos con los codos apenas flexionados y volver a juntar arriba."),
            ("Pecho", "Flexiones de brazos", "Manos al ancho de los hombros, cuerpo recto, bajar el pecho cerca del piso y subir."),

            // Espalda
            ("Espalda", "Dominadas", "Colgado de la barra, subir hasta pasar el mentón y bajar controlado."),
            ("Espalda", "Remo con barra", "Torso inclinado y espalda recta, llevar la barra hacia el abdomen."),
            ("Espalda", "Jalón al pecho", "Sentado en la polea, llevar la barra hacia la parte alta del pecho."),
            ("Espalda", "Remo con mancuerna", "Con una mano y una rodilla apoyadas en el banco, llevar la mancuerna hacia la cadera."),
            ("Espalda", "Peso muerto", "Levantar la barra desde el piso con la espalda recta, extendiendo cadera y rodillas."),

            // Piernas
            ("Piernas", "Sentadilla", "Con la barra sobre los hombros, bajar la cadera hasta que los muslos queden paralelos al piso."),
            ("Piernas", "Prensa de piernas", "Sentado en la máquina, empujar la plataforma sin trabar las rodillas."),
            ("Piernas", "Estocadas", "Dar un paso largo y bajar hasta que las dos rodillas queden a 90 grados."),
            ("Piernas", "Extensión de cuádriceps", "Sentado en la máquina, extender las piernas y bajar controlado."),
            ("Piernas", "Curl femoral", "En la máquina, flexionar las rodillas llevando el rodillo hacia los glúteos."),
            ("Piernas", "Elevación de talones", "De pie, subir en puntas de pie y bajar lento. Trabaja los gemelos."),

            // Hombros
            ("Hombros", "Press militar", "De pie o sentado, empujar la barra desde los hombros hasta arriba de la cabeza."),
            ("Hombros", "Elevaciones laterales", "Subir las mancuernas hacia los costados hasta la altura de los hombros."),
            ("Hombros", "Elevaciones frontales", "Subir las mancuernas hacia adelante hasta la altura de los hombros."),
            ("Hombros", "Pájaros", "Torso inclinado, abrir los brazos hacia los costados. Trabaja el hombro posterior."),

            // Bíceps
            ("Bíceps", "Curl con barra", "De pie, flexionar los codos llevando la barra hacia los hombros, sin balancear el cuerpo."),
            ("Bíceps", "Curl con mancuernas", "Flexionar los codos alternando los brazos, girando la palma hacia arriba."),
            ("Bíceps", "Curl martillo", "Curl con las palmas enfrentadas durante todo el movimiento."),
            ("Bíceps", "Curl en banco Scott", "Con los brazos apoyados en el banco, flexionar los codos de forma controlada."),

            // Tríceps
            ("Tríceps", "Fondos en paralelas", "Sostenido en las barras, bajar flexionando los codos y empujar hasta extender."),
            ("Tríceps", "Extensión de tríceps en polea", "Codos pegados al cuerpo, empujar la barra hacia abajo hasta extender los brazos."),
            ("Tríceps", "Press francés", "Acostado, bajar la barra hacia la frente flexionando solo los codos y extender."),
            ("Tríceps", "Patada de tríceps", "Torso inclinado, extender el codo llevando la mancuerna hacia atrás."),

            // Abdominales
            ("Abdominales", "Plancha", "Apoyado en antebrazos y puntas de pie, mantener el cuerpo recto. Las repeticiones son segundos."),
            ("Abdominales", "Crunch abdominal", "Acostado boca arriba, despegar los hombros del piso contrayendo el abdomen."),
            ("Abdominales", "Elevación de piernas", "Acostado o colgado, subir las piernas extendidas y bajar sin apoyar."),
            ("Abdominales", "Giros rusos", "Sentado con el torso inclinado, girar llevando las manos de un lado al otro."),

            // Glúteos
            ("Glúteos", "Hip thrust", "Espalda apoyada en el banco y barra sobre la cadera, empujar la cadera hacia arriba."),
            ("Glúteos", "Puente de glúteos", "Acostado boca arriba con las rodillas flexionadas, elevar la cadera y apretar arriba."),
            ("Glúteos", "Patada de glúteo", "En cuatro apoyos o en polea, llevar la pierna hacia atrás y arriba."),
            ("Glúteos", "Sentadilla búlgara", "Con un pie apoyado atrás en el banco, bajar con la pierna de adelante.")
        };

        public static void CargarEjercicios(AppDbContext db)
        {
            var grupos = db.GruposMusculares.ToDictionary(g => g.Nombre, g => g.Id);

            var existentes = db.Ejercicios
                .Select(e => e.Nombre)
                .ToList()
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var (grupo, nombre, descripcion) in Catalogo)
            {
                if (existentes.Contains(nombre) || !grupos.TryGetValue(grupo, out var grupoId))
                {
                    continue;
                }

                db.Ejercicios.Add(new Ejercicio
                {
                    Nombre = nombre,
                    Descripcion = descripcion,
                    GrupoMuscularId = grupoId,
                    // Búsqueda en YouTube de la técnica: el profesor puede cambiarla por un video puntual
                    VideoUrl = "https://www.youtube.com/results?search_query="
                        + Uri.EscapeDataString(nombre + " técnica correcta")
                });
            }

            db.SaveChanges();
        }
    }
}
