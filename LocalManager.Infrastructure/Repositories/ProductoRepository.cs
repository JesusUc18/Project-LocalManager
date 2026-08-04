// ============================================================
// ARCHIVO: ProductoRepository.cs
// RUTA:    LocalManager.Infrastructure/Repositories/ProductoRepository.cs
// ============================================================
// REEMPLAZA el archivo existente completo con este contenido.
// ============================================================

using LocalManager.Domain.Entities;
using LocalManager.Domain.Interfaces.Repositories;
using LocalManager.Infrastructure.Data;

namespace LocalManager.Infrastructure.Repositories
{
    /// <summary>
    /// PATRÓN REPOSITORY (GOF - Estructural) — ADR-05
    /// Implementa IProductoRepository usando IDbContext (Strategy).
    /// 
    /// CAMBIO vs versión anterior: recibe IDbContext en lugar de JsonDbContext.
    /// Esto permite intercambiar la estrategia de persistencia sin modificar este archivo.
    /// CAPA: Infrastructure — implementa la interfaz definida en Domain.
    /// </summary>
    public class ProductoRepository : IProductoRepository
    {
        private readonly IDbContext _context; // ← IDbContext en lugar de JsonDbContext

        public ProductoRepository(IDbContext context)
        {
            _context = context;
        }

        public List<Producto> ObtenerTodos()
        {
            var productos = _context.Set<Producto>();
            var categorias = _context.Set<Categoria>().ToDictionary(c => c.Id, c => c);
            foreach (var p in productos)
            {
                if (categorias.TryGetValue(p.CategoriaId, out var cat))
                    p.Categoria = cat;
            }
            return productos;
        }
        public Producto? ObtenerPorId(int id)
        {
            var productos = ObtenerTodos(); // reutiliza el método que ya carga categorías
            return productos.FirstOrDefault(p => p.Id == id);
        }

        public void Agregar(Producto producto)
        {
            _context.Add(producto);
            _context.SaveChanges();
        }

        public void Actualizar(Producto producto)
        {
            producto.Categoria = null;
            _context.Update(producto);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            _context.Remove<Producto>(id);
            _context.SaveChanges();
        }
    }
}