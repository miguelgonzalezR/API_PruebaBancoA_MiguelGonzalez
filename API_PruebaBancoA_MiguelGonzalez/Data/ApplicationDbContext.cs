using API_PruebaBancoA_MiguelGonzalez.Models;
using Microsoft.EntityFrameworkCore;

namespace API_PruebaBancoA_MiguelGonzalez.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            Tarjetas = Set<Tarjetas>();
            Compras = Set<Compras>();
            Pago = Set<Pagos>();
        }

        public DbSet<Tarjetas> Tarjetas { get; set; }
        public DbSet<Compras> Compras { get; set; }
        public DbSet<Pagos> Pago { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Compras>()
                .HasOne(c => c.Tarjeta)
                .WithMany(t => t.Compras)
                .HasForeignKey(c => c.TarjetaId);

            modelBuilder.Entity<Pagos>()
               .HasOne(p => p.Tarjeta)
               .WithMany(t => t.Pagos)
               .HasForeignKey(p => p.TarjetaId);
        }
    }
}
