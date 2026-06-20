using LocalManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalManager.Infrastructure.Data
{
    /// <summary>
    /// DbContext de Entity Framework Core para SQL Server.
    /// VISTA DE DESARROLLO (ADR-02): Capa Infrastructure → Acceso a datos
    /// VISTA DE DESPLIEGUE (ADR-02): Conecta la aplicación con SQL Server
    /// 
    /// Descomentar en Program.cs cuando se migre de JSON a base de datos relacional.
    /// CAPA: Infrastructure — implementa la persistencia, depende de Domain (entidades).
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Venta> Ventas { get; set; } = null!;
        public DbSet<DetalleVenta> DetalleVentas { get; set; } = null!;
        public DbSet<Caja> Cajas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Venta>()
                .HasMany(v => v.Detalles)
                .WithOne(d => d.Venta)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Caja>()
                .HasMany(c => c.Ventas)
                .WithOne(v => v.Caja)
                .HasForeignKey(v => v.CajaId);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.CategoriaId);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany()
                .HasForeignKey(v => v.ClienteId);

            modelBuilder.Entity<Producto>().Property(p => p.Precio).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DetalleVenta>().Property(d => d.PrecioUnitario).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Caja>().Property(c => c.MontoInicial).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Caja>().Property(c => c.MontoCierre).HasColumnType("decimal(18,2)");
        }
    }
}
