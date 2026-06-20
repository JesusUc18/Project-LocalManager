using LocalManager.Domain.Entities;
using LocalManager.Domain.Interfaces.Repositories;
using LocalManager.Infrastructure.Data;

namespace LocalManager.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de categorías usando JSON.
    /// CAPA: Infrastructure — implementa la interfaz definida en Domain.
    /// </summary>
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly JsonDbContext _context;

        public CategoriaRepository(JsonDbContext context)
        {
            _context = context;
        }

        public List<Categoria> ObtenerTodas() => _context.Set<Categoria>();
        public Categoria? ObtenerPorId(int id) => _context.Find<Categoria>(id);

        public void Agregar(Categoria categoria)
        {
            _context.Add(categoria);
            _context.SaveChanges();
        }

        public void Actualizar(Categoria categoria)
        {
            _context.Update(categoria);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            _context.Remove<Categoria>(id);
            _context.SaveChanges();
        }
    }
}
