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
    public class ClienteRepository : IClienteRepository
    {
        private readonly IDbContext _context;

        public ClienteRepository(IDbContext context)
        {
            _context = context;
        }

        public List<Cliente> ObtenerTodos() => _context.Set<Cliente>();
        public Cliente? ObtenerPorId(int id) => _context.Find<Cliente>(id);

        public void Agregar(Cliente cliente)
        {
            _context.Add(cliente);
            _context.SaveChanges();
        }

        public void Actualizar(Cliente cliente)
        {
            _context.Update(cliente);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            _context.Remove<Cliente>(id);
            _context.SaveChanges();
        }
    }
}