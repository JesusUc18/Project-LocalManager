using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Presentation.Controllers
{
    /// <summary>
    /// CAPA: Presentation — depende de Application (IClienteService).
    /// ADR-03: Arquitectura en Capas.
    /// </summary>
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        public IActionResult Index() => View(_clienteService.ObtenerTodos());
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _clienteService.Crear(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public IActionResult Edit(int id)
        {
            var cliente = _clienteService.ObtenerPorId(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _clienteService.Actualizar(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public IActionResult Delete(int id)
        {
            var cliente = _clienteService.ObtenerPorId(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _clienteService.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
