using Microsoft.EntityFrameworkCore;

namespace LocalManager.Infrastructure.Data
{
    /// <summary>
    /// PATRÓN STRATEGY (GOF - Comportamiento) — ADR-05
    /// Estrategia concreta: persistencia en PostgreSQL usando Entity Framework Core.
    /// Implementa IDbContext para ser intercambiable con JsonDbContext.
    ///
    /// CAPA: Infrastructure — implementa la persistencia, depende de Domain (entidades)
    /// y de AppDbContext (configuración de EF Core / Npgsql).
    /// </summary>
    public class SqlDbContext : IDbContext
    {
        private readonly AppDbContext _context;

        public SqlDbContext(AppDbContext context)
        {
            _context = context;
        }

        public List<T> Set<T>() where T : class => _context.Set<T>().AsNoTracking().ToList();

        public void Add<T>(T entity) where T : class => _context.Set<T>().Add(entity);

        public void Update<T>(T entity) where T : class
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove<T>(int id) where T : class
        {
            var entity = _context.Set<T>().Find(id);
            if (entity != null)
                _context.Set<T>().Remove(entity);
        }

        public T? Find<T>(int id) where T : class => _context.Set<T>().Find(id);

        public int SaveChanges() => _context.SaveChanges();
    }
}