using Microsoft.EntityFrameworkCore;
using LeonoraInventory.Models;

namespace LeonoraInventory.Services
{
    public class InventarioContext : DbContext
    {
        // Constructor para inyección / migraciones
        public InventarioContext(DbContextOptions<InventarioContext> options)
            : base(options)
        {
        }

        // Constructor parameterless
        public InventarioContext()
        {
        }

        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Movimiento> Movimientos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Solo se ejecuta si no se ha llamado al constructor con options
            if (!options.IsConfigured)
            {
                options.UseSqlite("Data Source=Inventario.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>()
                .HasMany(c => c.Productos)
                .WithOne(p => p.Categoria)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Producto>()
                .HasMany(p => p.Movimientos)
                .WithOne(m => m.Producto)
                .HasForeignKey(m => m.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
