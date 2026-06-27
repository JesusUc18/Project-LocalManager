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
    public class CajaRepository : ICajaRepository
    {
        private readonly IDbContext _context;

        public CajaRepository(IDbContext context)
        {
            _context = context;
        }

        public List<Caja> ObtenerTodas()
        {
            var cajas = _context.Set<Caja>();
            var ventas = _context.Set<Venta>();
            var detalles = _context.Set<DetalleVenta>();
            foreach (var caja in cajas)
            {
                caja.Ventas = ventas.Where(v => v.CajaId == caja.Id).ToList();
                foreach (var v in caja.Ventas)
                    v.Detalles = detalles.Where(d => d.VentaId == v.Id).ToList();
            }
            return cajas;
        }

        public Caja? ObtenerPorId(int id) => ObtenerTodas().FirstOrDefault(c => c.Id == id);
        public List<Caja> ObtenerAbiertas() => ObtenerTodas().Where(c => c.Estado == "Abierta").ToList();

        public void Agregar(Caja caja)
        {
            _context.Add(caja);
            _context.SaveChanges();
        }

        public void Actualizar(Caja caja)
        {
            _context.Update(caja);
            _context.SaveChanges();
        }
    }
}