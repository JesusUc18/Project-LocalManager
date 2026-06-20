using LocalManager.Domain.Entities;
using LocalManager.Domain.Interfaces.Repositories;
using LocalManager.Infrastructure.Data;

namespace LocalManager.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de cajas usando JSON.
    /// CAPA: Infrastructure — implementa la interfaz definida en Domain.
    /// </summary>
    public class CajaRepository : ICajaRepository
    {
        private readonly JsonDbContext _context;

        public CajaRepository(JsonDbContext context)
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
