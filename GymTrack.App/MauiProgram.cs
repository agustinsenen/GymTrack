using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using GymTrack.App.Services;

namespace GymTrack.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // API publicada en MonsterASP
            const string urlApi = "https://gymtracks.runasp.net/";

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(urlApi) });

            // Sesión
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ApiClient>();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<AuthenticationStateProvider, SesionAuthStateProvider>();

            // Un servicio por cada parte de la API
            builder.Services.AddSingleton<AlumnosService>();
            builder.Services.AddSingleton<ProfesoresService>();
            builder.Services.AddSingleton<EjerciciosService>();
            builder.Services.AddSingleton<RutinasService>();
            builder.Services.AddSingleton<ProgresoService>();

            return builder.Build();
        }
    }
}
