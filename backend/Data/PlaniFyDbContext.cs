using Microsoft.EntityFrameworkCore;
using backend_lab_c28730.Models;

namespace backend_lab_c28730.Data
{
    public class PlaniFyDbContext : DbContext
    {
        public PlaniFyDbContext(DbContextOptions<PlaniFyDbContext> options) : base(options)
        {
        }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between Persona and Empleado
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Persona)
                .WithOne(p => p.Empleado)
                .HasForeignKey<Empleado>(e => e.idPersona)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between Persona and Direccion
            modelBuilder.Entity<Persona>()
                .HasOne(p => p.Direccion)
                .WithMany(d => d.Personas)
                .HasForeignKey(p => p.idDireccion)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure the relationship between Empleado and Empresa
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Empresa)
                .WithMany(emp => emp.Empleados)
                .HasForeignKey(e => e.idEmpresa)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
