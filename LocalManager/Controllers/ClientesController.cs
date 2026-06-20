using LocalManager.Models;
using LocalManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ClienteService _clienteService;

        public ClientesController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        public IActionResult Index()
        {
            return View(_clienteService.GetAll());
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _clienteService.Add(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public IActionResult Edit(int id)
        {
            var cliente = _clienteService.GetById(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        public IActionResult Edit(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _clienteService.Update(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public IActionResult Delete(int id)
        {
            var cliente = _clienteService.GetById(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _clienteService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
