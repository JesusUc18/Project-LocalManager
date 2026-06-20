using LocalManager.Api.Models;
using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Api.Controllers
{
    /// <summary>
    /// Endpoints para gestionar clientes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesApiController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesApiController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Obtiene todos los clientes.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Cliente>), 200)]
        public IActionResult GetAll()
        {
            var clientes = _clienteService.ObtenerTodos();
            return Ok(new ApiResponse<List<Cliente>> { Exito = true, Datos = clientes });
        }

        /// <summary>
        /// Obtiene un cliente por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Cliente), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetById(int id)
        {
            var cliente = _clienteService.ObtenerPorId(id);
            if (cliente == null) return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Cliente no encontrado" });
            return Ok(new ApiResponse<Cliente> { Exito = true, Datos = cliente });
        }

        /// <summary>
        /// Crea un nuevo cliente.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Cliente), 201)]
        public IActionResult Create([FromBody] Cliente cliente)
        {
            _clienteService.Crear(cliente);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, new ApiResponse<Cliente> { Exito = true, Mensaje = "Cliente creado", Datos = cliente });
        }

        /// <summary>
        /// Actualiza un cliente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Update(int id, [FromBody] Cliente cliente)
        {
            if (_clienteService.ObtenerPorId(id) == null)
                return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Cliente no encontrado" });
            cliente.Id = id;
            _clienteService.Actualizar(cliente);
            return NoContent();
        }

        /// <summary>
        /// Elimina un cliente.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int id)
        {
            if (_clienteService.ObtenerPorId(id) == null)
                return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Cliente no encontrado" });
            _clienteService.Eliminar(id);
            return NoContent();
        }
    }
}
