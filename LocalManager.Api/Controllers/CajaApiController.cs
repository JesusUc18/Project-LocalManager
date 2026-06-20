using LocalManager.Api.Models;
using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Api.Controllers
{
    /// <summary>
    /// Endpoints para gestionar turnos de caja.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CajaApiController : ControllerBase
    {
        private readonly ICajaService _cajaService;

        public CajaApiController(ICajaService cajaService)
        {
            _cajaService = cajaService;
        }

        /// <summary>
        /// Obtiene todas las cajas registradas.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Caja>), 200)]
        public IActionResult GetAll()
        {
            var cajas = _cajaService.ObtenerTodas();
            return Ok(new ApiResponse<List<Caja>> { Exito = true, Datos = cajas });
        }

        /// <summary>
        /// Obtiene una caja por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Caja), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetById(int id)
        {
            var caja = _cajaService.ObtenerPorId(id);
            if (caja == null) return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Caja no encontrada" });
            return Ok(new ApiResponse<Caja> { Exito = true, Datos = caja });
        }

        /// <summary>
        /// Obtiene las cajas actualmente abiertas.
        /// </summary>
        [HttpGet("abiertas")]
        [ProducesResponseType(typeof(List<Caja>), 200)]
        public IActionResult GetAbiertas()
        {
            var cajas = _cajaService.ObtenerAbiertas();
            return Ok(new ApiResponse<List<Caja>> { Exito = true, Datos = cajas });
        }

        /// <summary>
        /// Abre un nuevo turno de caja.
        /// </summary>
        [HttpPost("abrir")]
        [ProducesResponseType(typeof(Caja), 201)]
        public IActionResult Abrir([FromBody] AbrirCajaRequest request)
        {
            var caja = new Caja
            {
                MontoInicial = request.MontoInicial,
                Responsable = request.Responsable
            };
            _cajaService.Abrir(caja);
            return CreatedAtAction(nameof(GetById), new { id = caja.Id }, new ApiResponse<Caja> { Exito = true, Mensaje = "Caja abierta", Datos = caja });
        }

        /// <summary>
        /// Cierra un turno de caja.
        /// </summary>
        [HttpPost("{id}/cerrar")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Cerrar(int id, [FromBody] CerrarCajaRequest request)
        {
            if (_cajaService.ObtenerPorId(id) == null)
                return NotFound(new ApiResponse<object> { Exito = false, Mensaje = "Caja no encontrada" });
            _cajaService.Cerrar(id, request.MontoCierre);
            return NoContent();
        }
    }
}
