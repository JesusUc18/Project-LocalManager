using LocalManager.Api.Models;
using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Api.Controllers
{
    /// <summary>
    /// Endpoints para gestionar ventas y transacciones.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VentasApiController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentasApiController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        /// <summary>
        /// Obtiene todas las ventas registradas.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Venta>), 200)]
        public IActionResult GetAll()
        {
            var ventas = _ventaService.ObtenerTodas();
            return Ok(new ApiResponse<List<Venta>> { Exito = true, Datos = ventas });
        }

        /// <summary>
        /// Obtiene una venta por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Venta), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetById(int id)
        {
            var venta = _ventaService.ObtenerPorId(id);
            if (venta == null) return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Venta no encontrada" });
            return Ok(new ApiResponse<Venta> { Exito = true, Datos = venta });
        }

        /// <summary>
        /// Registra una nueva venta con múltiples productos.
        /// Operación transaccional: si no hay stock, la venta completa se cancela.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Venta), 201)]
        [ProducesResponseType(400)]
        public IActionResult Create([FromBody] CrearVentaRequest request)
        {
            var venta = new Venta
            {
                ClienteId = request.ClienteId,
                CajaId = request.CajaId,
                MetodoPago = request.MetodoPago,
                Detalles = request.Detalles.Select(d => new DetalleVenta
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad
                }).ToList()
            };

            var (exito, mensaje) = _ventaService.Registrar(venta);
            if (!exito)
                return BadRequest(new ApiResponse<object> { Exito = false, Mensaje = mensaje });

            return CreatedAtAction(nameof(GetById), new { id = venta.Id }, new ApiResponse<Venta> { Exito = true, Mensaje = mensaje, Datos = venta });
        }

        /// <summary>
        /// Obtiene las ventas de una caja específica.
        /// </summary>
        [HttpGet("caja/{cajaId}")]
        [ProducesResponseType(typeof(List<Venta>), 200)]
        public IActionResult GetByCaja(int cajaId)
        {
            var ventas = _ventaService.ObtenerPorCaja(cajaId);
            return Ok(new ApiResponse<List<Venta>> { Exito = true, Datos = ventas });
        }

        /// <summary>
        /// Obtiene las ventas de una fecha específica.
        /// </summary>
        [HttpGet("fecha/{fecha:datetime}")]
        [ProducesResponseType(typeof(List<Venta>), 200)]
        public IActionResult GetByFecha(DateTime fecha)
        {
            var ventas = _ventaService.ObtenerPorFecha(fecha);
            return Ok(new ApiResponse<List<Venta>> { Exito = true, Datos = ventas });
        }
    }
}
