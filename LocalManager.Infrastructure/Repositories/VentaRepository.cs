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
    public class VentaRepository : IVentaRepository
    {
        private readonly IDbContext _context;

        public VentaRepository(IDbContext context)
        {
            _context = context;
        }

        public List<Venta> ObtenerTodas()
        {
            var ventas = _context.Set<Venta>();
            var detalles = _context.Set<DetalleVenta>();
            var clientes = _context.Set<Cliente>().ToDictionary(c => c.Id, c => c.Nombre);
            foreach (var v in ventas)
            {
                v.Detalles = detalles.Where(d => d.VentaId == v.Id).ToList();
                if (v.ClienteId.HasValue && clientes.ContainsKey(v.ClienteId.Value))
                    v.Cliente = new Cliente { Id = v.ClienteId.Value, Nombre = clientes[v.ClienteId.Value] };
            }
            return ventas;
        }

        public Venta? ObtenerPorId(int id) => ObtenerTodas().FirstOrDefault(v => v.Id == id);

        public void Agregar(Venta venta)
        {
            _context.Add(venta);
            _context.SaveChanges();
        }

        public void AgregarDetalle(DetalleVenta detalle)
        {
            _context.Add(detalle);
            _context.SaveChanges();
        }

        public List<Venta> ObtenerPorCaja(int cajaId) => ObtenerTodas().Where(v => v.CajaId == cajaId).ToList();
        public List<Venta> ObtenerPorFecha(DateTime fecha) => ObtenerTodas().Where(v => v.Fecha.Date == fecha.Date).ToList();
    }
}