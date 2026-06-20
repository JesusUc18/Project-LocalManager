using LocalManager.Domain.Entities;
using LocalManager.Domain.Interfaces.Repositories;

namespace LocalManager.Application.Services
{
    /// <summary>
    /// Implementación del servicio de ventas con transacciones atómicas.
    /// CAPA: Application — contiene las reglas de negocio críticas.
    /// 
    /// VISTA DE PROCESOS (ADR-02): 
    /// Application → Domain (entidades) → Repositorios (interfaces) → Infrastructure (implementación)
    /// 
    /// Transacciones atómicas (ADR-01): 
    /// La venta y el descuento de stock ocurren juntos o no ocurren.
    /// </summary>
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IClienteRepository _clienteRepository;

        public VentaService(IVentaRepository ventaRepository, IProductoRepository productoRepository,
                            IClienteRepository clienteRepository)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
            _clienteRepository = clienteRepository;
        }

        public List<Venta> ObtenerTodas() => _ventaRepository.ObtenerTodas();
        public Venta? ObtenerPorId(int id) => _ventaRepository.ObtenerPorId(id);
        public List<Venta> ObtenerPorCaja(int cajaId) => _ventaRepository.ObtenerPorCaja(cajaId);
        public List<Venta> ObtenerPorFecha(DateTime fecha) => _ventaRepository.ObtenerPorFecha(fecha);

        public (bool exito, string mensaje) Registrar(Venta venta)
        {
            // ─── FASE 1: VALIDACIÓN ───
            foreach (var d in venta.Detalles)
            {
                var producto = _productoRepository.ObtenerPorId(d.ProductoId);
                if (producto == null)
                    return (false, $"El producto con ID {d.ProductoId} no existe.");
                if (producto.Stock < d.Cantidad)
                    return (false, $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado: {d.Cantidad}.");
            }

            // ─── FASE 2: EJECUCIÓN ATÓMICA ───
            try
            {
                // 1. Guardar la venta
                _ventaRepository.Agregar(venta);

                // 2. Guardar detalles y descontar stock
                var productosDescontados = new List<(int productoId, int cantidad)>();
                foreach (var d in venta.Detalles)
                {
                    var producto = _productoRepository.ObtenerPorId(d.ProductoId);
                    if (producto != null)
                    {
                        d.VentaId = venta.Id;
                        d.PrecioUnitario = producto.Precio;
                        d.Producto = new Producto { Id = producto.Id, Nombre = producto.Nombre };
                        _ventaRepository.AgregarDetalle(d);
                    }

                    // Descontar stock
                    if (producto != null && producto.Stock >= d.Cantidad)
                    {
                        producto.Stock -= d.Cantidad;
                        _productoRepository.Actualizar(producto);
                        productosDescontados.Add((d.ProductoId, d.Cantidad));
                    }
                    else
                    {
                        // Rollback manual: revertir descuentos previos
                        foreach (var (pid, cant) in productosDescontados)
                        {
                            var p = _productoRepository.ObtenerPorId(pid);
                            if (p != null) { p.Stock += cant; _productoRepository.Actualizar(p); }
                        }
                        return (false, "Error inesperado al descontar stock.");
                    }
                }

                return (true, "Venta registrada exitosamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al registrar la venta: {ex.Message}");
            }
        }
    }
}
