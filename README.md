# GymTrack

Trabajo final de Programación Móvil: aplicación para la gestión interna de un gimnasio.
Conecta al administrador, a los profesores y a los alumnos: rutinas por día, registro de pesos,
asistencia y control de cuotas.

Autor: Agustín Senén González

## Qué incluye

| Proyecto | Tecnología | Descripción |
|---|---|---|
| `GymTrack.API` | ASP.NET Core 8, Entity Framework Core, SQL Server, JWT | API REST con controladores, servicios e interfaces |
| `GymTrack.App` | .NET MAUI Blazor Hybrid (Android) | Aplicación móvil que consume la API |

La API y la base de datos están publicadas en MonsterASP.NET:

- API: https://gymtracks.runasp.net
- Documentación (Swagger): https://gymtracks.runasp.net/swagger

## Funcionalidades por rol

**Administrador**
- Alta, baja y modificación de profesores y alumnos.
- Reasignación de alumnos a otro profesor.
- Panel con totales de alumnos, profesores y cuotas vencidas.

**Profesor**
- Alta, baja y modificación de sus alumnos (se identifican por DNI).
- Catálogo de ejercicios con grupo muscular, foto y video de referencia.
- Rutina semanal de cada alumno, organizada por día.
- Registro de pagos de cuota con fecha de pago y nuevo vencimiento.
- Consulta del progreso de pesos y de la asistencia de cada alumno.

**Alumno**
- Rutina del día con la ficha de cada ejercicio (foto, video, series y repeticiones).
- Registro de los kg utilizados y botón "Marcar como realizado".
- Gráfico de evolución de pesos por ejercicio.
- Calendario con los días de asistencia y el vencimiento de la cuota.

## Base de datos

12 tablas normalizadas (3FN): Roles, Usuarios, Alumnos, GruposMusculares, Ejercicios, Rutinas,
RutinaEjercicios, HistorialProgresos, Asistencias, PagosCuota, Auditorias y LogsError.

Las tablas se crean solas con las migraciones de Entity Framework al iniciar la API, junto con
los roles, los grupos musculares, un catálogo base de ejercicios y el usuario administrador.

## Seguridad

- Autenticación con JWT y autorización por rol en cada endpoint.
- Contraseñas guardadas con hash BCrypt.
- Un profesor solo puede ver y modificar a sus propios alumnos.
- Auditoría de las acciones importantes y registro de errores en la base de datos.

## Cómo ejecutar

**API (local)**
1. Abrir `GymTrack.sln` en Visual Studio 2022.
2. En la Consola del Administrador de Paquetes: `Update-Database`.
3. Ejecutar `GymTrack.API`. Se abre Swagger.

**App**
1. Elegir `GymTrack.App` como proyecto de inicio y un emulador Android.
2. Ejecutar. La app usa la API publicada (la dirección está en `MauiProgram.cs`).

## Usuario inicial

| Rol | Usuario | Contraseña |
|---|---|---|
| Administrador | admin@gymtrack.com | Admin123! |

Los profesores los crea el administrador. Los alumnos los crea un profesor o el administrador;
si no se les carga una contraseña, la inicial es su DNI, y pueden ingresar con el DNI o con el email.
