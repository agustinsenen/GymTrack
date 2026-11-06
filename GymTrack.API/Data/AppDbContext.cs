using Microsoft.EntityFrameworkCore;
using GymTrack.API.Models;

namespace GymTrack.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Alumno> Alumnos { get; set; }

        public DbSet<GrupoMuscular> GruposMusculares { get; set; }

        public DbSet<Ejercicio> Ejercicios { get; set; }

        public DbSet<Rutina> Rutinas { get; set; }

        public DbSet<RutinaEjercicio> RutinaEjercicios { get; set; }

        public DbSet<HistorialProgreso> HistorialProgresos { get; set; }

        public DbSet<Asistencia> Asistencias { get; set; }

        public DbSet<PagoCuota> PagosCuota { get; set; }

        public DbSet<Auditoria> Auditorias { get; set; }

        public DbSet<LogError> LogsError { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Únicos ---
            modelBuilder.Entity<Rol>().HasIndex(r => r.Nombre).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Alumno>().HasIndex(a => a.Dni).IsUnique();
            modelBuilder.Entity<GrupoMuscular>().HasIndex(g => g.Nombre).IsUnique();

            // Una sola asistencia por alumno y por día
            modelBuilder.Entity<Asistencia>()
                .HasIndex(a => new { a.AlumnoId, a.Fecha })
                .IsUnique();

            // --- Alumno tiene DOS relaciones con Usuario, hay que aclarar cuál es cuál ---
            // 1:1 -> el usuario que ES el alumno
            modelBuilder.Entity<Alumno>()
                .HasOne(a => a.Usuario)
                .WithOne(u => u.Alumno)
                .HasForeignKey<Alumno>(a => a.UsuarioId);

            // 1:N -> el profesor que lo tiene a cargo
            modelBuilder.Entity<Alumno>()
                .HasOne(a => a.Profesor)
                .WithMany(u => u.AlumnosACargo)
                .HasForeignKey(a => a.ProfesorId);

            // --- Columnas solo fecha ---
            modelBuilder.Entity<Asistencia>().Property(a => a.Fecha).HasColumnType("date");
            modelBuilder.Entity<PagoCuota>().Property(p => p.FechaPago).HasColumnType("date");
            modelBuilder.Entity<PagoCuota>().Property(p => p.NuevaFechaVencimiento).HasColumnType("date");

            // --- Precisión de los kg ---
            modelBuilder.Entity<HistorialProgreso>().Property(h => h.PesoKg).HasPrecision(6, 2);
            modelBuilder.Entity<RutinaEjercicio>().Property(r => r.KgObjetivo).HasPrecision(6, 2);

            // Ninguna eliminación borra datos en cascada (igual que en el TP5)
            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
