namespace LocalManager.Infrastructure.Data
{
    /// <summary>
    /// PATRÓN STRATEGY (GOF - Comportamiento) — ADR-05
    /// Interfaz que define el contrato de persistencia intercambiable.
    /// 
    /// Estrategias concretas:
    ///   - JsonDbContext  : persistencia en archivos JSON (desarrollo/actual)
    ///   - SqlDbContext   : persistencia en SQL Server con EF Core (producción/futuro)
    /// 
    /// Los repositorios dependen de esta interfaz, nunca de una implementación concreta.
    /// Para cambiar de JSON a SQL Server basta con modificar el registro en Program.cs.
    /// </summary>
    public interface IDbContext
    {
        List<T> Set<T>() where T : class;
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Remove<T>(int id) where T : class;
        T? Find<T>(int id) where T : class;
        int SaveChanges();
    }
}