using LocalManager.Domain.Entities;
using LocalManager.Domain.Interfaces.Repositories;
using LocalManager.Infrastructure.Data;

namespace LocalManager.Infrastructure.Repositories
{
    /// <summary>
    /// PATRÓN REPOSITORY (GOF - Estructural) — ADR-05
    /// CAMBIO: recibe IDbContext en lugar de JsonDbContext.
    /// CAPA: Infrastructure.
    /// </summary>
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly IDbContext _context;

        public CategoriaRepository(IDbContext context)
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