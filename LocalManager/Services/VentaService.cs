using LocalManager.Models;

namespace LocalManager.Services
{
    public class VentaService
    {
        private readonly JsonDatabaseService _db;
        private readonly ProductoService _productoService;

        public VentaService(JsonDatabaseService db, ProductoService productoService)
        {
            _db = db;
            _productoService = productoService;
        }

        public List<Venta> GetAll()
        {
            var ventas = _db.GetAll<Venta>();
            var detalles = _db.GetAll<DetalleVenta>();
            foreach (var v in ventas)
                v.Detalles = detalles.Where(d => d.VentaId == v.Id).ToList();
            return ventas;
        }

        public Venta? GetById(int id) => GetAll().FirstOrDefault(v => v.Id == id);

        public bool Registrar(Venta venta)
        {
            // Validar stock
            foreach (var d in venta.Detalles)
            {
                var producto = _productoService.GetById(d.ProductoId);
                if (producto == null || producto.Stock < d.Cantidad)
                    return false;
            }

            // Guardar venta
            _db.Add(venta);

            // Guardar detalles y descontar stock
            foreach (var d in venta.Detalles)
            {
                d.VentaId = venta.Id;
                var producto = _productoService.GetById(d.ProductoId);
                if (producto != null)
                {
                    d.ProductoNombre = producto.Nombre;
                    d.PrecioUnitario = producto.Precio;
                }
                _db.Add(d);
                _productoService.DescontarStock(d.ProductoId, d.Cantidad);
            }

            return true;
        }

        public List<Venta> GetByCaja(int cajaId) => GetAll().Where(v => v.CajaId == cajaId).ToList();
        public List<Venta> GetByDate(DateTime fecha) => GetAll().Where(v => v.Fecha.Date == fecha.Date).ToList();
    }
}
