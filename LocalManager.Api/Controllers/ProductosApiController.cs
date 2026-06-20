using LocalManager.Api.Models;
using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Api.Controllers
{
    /// <summary>
    /// Endpoints para gestionar productos del inventario.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosApiController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosApiController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        /// <summary>
        /// Obtiene todos los productos registrados.
        /// </summary>
        /// <returns>Lista de productos</returns>
        /// <response code="200">Lista de productos obtenida exitosamente</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Producto>), 200)]
        public IActionResult GetAll()
        {
            var productos = _productoService.ObtenerTodos();
            return Ok(new ApiResponse<List<Producto>> { Exito = true, Datos = productos });
        }

        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Producto encontrado</returns>
        /// <response code="200">Producto encontrado</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Producto), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetById(int id)
        {
            var producto = _productoService.ObtenerPorId(id);
            if (producto == null) return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Producto no encontrado" });
            return Ok(new ApiResponse<Producto> { Exito = true, Datos = producto });
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <param name="producto">Datos del producto</param>
        /// <returns>Producto creado</returns>
        /// <response code="201">Producto creado exitosamente</response>
        [HttpPost]
        [ProducesResponseType(typeof(Producto), 201)]
        public IActionResult Create([FromBody] Producto producto)
        {
            _productoService.Crear(producto);
            return CreatedAtAction(nameof(GetById), new { id = producto.Id }, new ApiResponse<Producto> { Exito = true, Mensaje = "Producto creado", Datos = producto });
        }

        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="producto">Datos actualizados</param>
        /// <response code="204">Producto actualizado</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Update(int id, [FromBody] Producto producto)
        {
            if (_productoService.ObtenerPorId(id) == null)
                return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Producto no encontrado" });
            producto.Id = id;
            _productoService.Actualizar(producto);
            return NoContent();
        }

        /// <summary>
        /// Elimina un producto.
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <response code="204">Producto eliminado</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int id)
        {
            if (_productoService.ObtenerPorId(id) == null)
                return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Producto no encontrado" });
            _productoService.Eliminar(id);
            return NoContent();
        }
    }
}
