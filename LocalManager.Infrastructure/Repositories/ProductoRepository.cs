using LocalManager.Domain.Entities;
using LocalManager.Domain.Interfaces.Repositories;
using LocalManager.Infrastructure.Data;

namespace LocalManager.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de productos usando JSON.
    /// CAPA: Infrastructure — implementa la interfaz definida en Domain.
    /// </summary>
    public class ProductoRepository : IProductoRepository
    {
        private readonly JsonDbContext _context;

        public ProductoRepository(JsonDbContext context)
        {
            _context = context;
        }

        public List<Producto> ObtenerTodos() => _context.Set<Producto>();
        public Producto? ObtenerPorId(int id) => _context.Find<Producto>(id);

        public void Agregar(Producto producto)
        {
            _context.Add(producto);
            _context.SaveChanges();
        }

        public void Actualizar(Producto producto)
        {
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
