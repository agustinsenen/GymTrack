using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using GymTrack.API.Data;
using GymTrack.API.Middleware;
using GymTrack.API.Models;
using GymTrack.API.Services;
using GymTrack.API.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaCORS", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var clave = builder.Configuration["Jwt:Key"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresá el token JWT."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Servicios (interfaz -> implementación) ---
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfesorService, ProfesorService>();
builder.Services.AddScoped<IAlumnoService, AlumnoService>();
builder.Services.AddScoped<ICuotaService, CuotaService>();
builder.Services.AddScoped<IResumenService, ResumenService>();
builder.Services.AddScoped<IImagenService, ImagenService>();
builder.Services.AddScoped<IEjercicioService, EjercicioService>();
builder.Services.AddScoped<IRutinaService, RutinaService>();
builder.Services.AddScoped<IProgresoService, ProgresoService>();

var app = builder.Build();

app.UseMiddleware<ManejoErroresMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("PoliticaCORS");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// --- Migraciones + datos iniciales ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.Migrate();

    // Roles
    foreach (var nombreRol in new[] { Rol.Administrador, Rol.Profesor, Rol.Alumno })
    {
        if (!db.Roles.Any(r => r.Nombre == nombreRol))
        {
            db.Roles.Add(new Rol { Nombre = nombreRol });
        }
    }
    db.SaveChanges();

    // Grupos musculares
    if (!db.GruposMusculares.Any())
    {
        db.GruposMusculares.AddRange(
            new[] { "Pecho", "Espalda", "Piernas", "Hombros", "Bíceps", "Tríceps", "Abdominales", "Glúteos" }
                .Select(n => new GrupoMuscular { Nombre = n }));
        db.SaveChanges();
    }

    // Catálogo base de ejercicios (solo agrega los que falten)
    DatosIniciales.CargarEjercicios(db);

    // Administrador inicial
    if (!db.Usuarios.Any(u => u.Email == "admin@gymtrack.com"))
    {
        db.Usuarios.Add(new Usuario
        {
            Nombre = "Administrador",
            Email = "admin@gymtrack.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            RolId = db.Roles.First(r => r.Nombre == Rol.Administrador).Id
        });
        db.SaveChanges();
    }
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
