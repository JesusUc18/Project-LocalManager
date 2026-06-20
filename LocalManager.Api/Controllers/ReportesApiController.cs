using LocalManager.Api.Models;
using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Api.Controllers
{
    /// <summary>
    /// Endpoints para consultar reportes del negocio.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesApiController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        private readonly IProductoService _productoService;
        private readonly ICajaService _cajaService;

        public ReportesApiController(IVentaService ventaService, IProductoService productoService, ICajaService cajaService)
        {
            _ventaService = ventaService;
            _productoService = productoService;
            _cajaService = cajaService;
        }

        /// <summary>
        /// Obtiene las ventas del día de hoy.
        /// </summary>
        [HttpGet("ventas-hoy")]
        [ProducesResponseType(typeof(List<Venta>), 200)]
        public IActionResult VentasHoy()
        {
            var ventas = _ventaService.ObtenerPorFecha(DateTime.Today);
            return Ok(new ApiResponse<List<Venta>> { Exito = true, Datos = ventas, Mensaje = $"{ventas.Count} ventas hoy" });
        }

        /// <summary>
        /// Obtiene las ventas del mes actual.
        /// </summary>
        [HttpGet("ventas-mes")]
        [ProducesResponseType(typeof(List<Venta>), 200)]
        public IActionResult VentasMes()
        {
            var ventas = _ventaService.ObtenerTodas()
                .Where(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year).ToList();
            return Ok(new ApiResponse<List<Venta>> { Exito = true, Datos = ventas, Mensaje = $"{ventas.Count} ventas este mes" });
        }

        /// <summary>
        /// Obtiene los productos con stock bajo (menor a 5 unidades).
        /// </summary>
        [HttpGet("stock-bajo")]
        [ProducesResponseType(typeof(List<Producto>), 200)]
        public IActionResult StockBajo()
        {
            var productos = _productoService.ObtenerTodos().Where(p => p.Stock < 5).ToList();
            return Ok(new ApiResponse<List<Producto>> { Exito = true, Datos = productos, Mensaje = $"{productos.Count} productos con stock bajo" });
        }

        /// <summary>
        /// Obtiene el resumen de todas las cajas.
        /// </summary>
        [HttpGet("resumen-cajas")]
        [ProducesResponseType(typeof(List<Caja>), 200)]
        public IActionResult ResumenCajas()
        {
            var cajas = _cajaService.ObtenerTodas();
            return Ok(new ApiResponse<List<Caja>> { Exito = true, Datos = cajas });
        }

        /// <summary>
        /// Obtiene el dashboard con KPIs del negocio.
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult Dashboard()
        {
            var productos = _productoService.ObtenerTodos();
            var ventas = _ventaService.ObtenerTodas();
            var cajasAbiertas = _cajaService.ObtenerAbiertas();

            var dashboard = new
            {
                TotalProductos = productos.Count,
                ProductosBajosStock = productos.Count(p => p.Stock < 5),
                VentasHoy = ventas.Count(v => v.Fecha.Date == DateTime.Today),
                TotalVentasHoy = ventas.Where(v => v.Fecha.Date == DateTime.Today).Sum(v => v.Total),
                CajasAbiertas = cajasAbiertas.Count
            };

            return Ok(new ApiResponse<object> { Exito = true, Datos = dashboard });
        }
    }
}
