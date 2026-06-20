using LocalManager.Api.Models;
using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Api.Controllers
{
    /// <summary>
    /// Endpoints para gestionar categorías de productos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasApiController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasApiController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        /// <summary>
        /// Obtiene todas las categorías.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Categoria>), 200)]
        public IActionResult GetAll()
        {
            var categorias = _categoriaService.ObtenerTodas();
            return Ok(new ApiResponse<List<Categoria>> { Exito = true, Datos = categorias });
        }

        /// <summary>
        /// Obtiene una categoría por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Categoria), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetById(int id)
        {
            var categoria = _categoriaService.ObtenerPorId(id);
            if (categoria == null) return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Categoría no encontrada" });
            return Ok(new ApiResponse<Categoria> { Exito = true, Datos = categoria });
        }

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Categoria), 201)]
        public IActionResult Create([FromBody] Categoria categoria)
        {
            _categoriaService.Crear(categoria);
            return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, new ApiResponse<Categoria> { Exito = true, Mensaje = "Categoría creada", Datos = categoria });
        }

        /// <summary>
        /// Actualiza una categoría.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Update(int id, [FromBody] Categoria categoria)
        {
            if (_categoriaService.ObtenerPorId(id) == null)
                return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Categoría no encontrada" });
            categoria.Id = id;
            _categoriaService.Actualizar(categoria);
            return NoContent();
        }

        /// <summary>
        /// Elimina una categoría.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int id)
        {
            if (_categoriaService.ObtenerPorId(id) == null)
                return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Categoría no encontrada" });
            _categoriaService.Eliminar(id);
            return NoContent();
        }
    }
}
