using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Presentation.Controllers
{
    /// <summary>
    /// CAPA: Presentation — depende de Application (ICajaService).
    /// ADR-03: Arquitectura en Capas.
    /// </summary>
    public class CajaController : Controller
    {
        private readonly ICajaService _cajaService;

        public CajaController(ICajaService cajaService)
        {
            _cajaService = cajaService;
        }

        public IActionResult Index() => View(_cajaService.ObtenerTodas());
        public IActionResult Abrir() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Abrir(Caja caja)
        {
            if (ModelState.IsValid)
            {
                _cajaService.Abrir(caja);
                return RedirectToAction(nameof(Index));
            }
            return View(caja);
        }

        public IActionResult Cerrar(int id)
        {
            var caja = _cajaService.ObtenerPorId(id);
            if (caja == null) return NotFound();
            return View(caja);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cerrar(int id, decimal montoCierre)
        {
            _cajaService.Cerrar(id, montoCierre);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var caja = _cajaService.ObtenerPorId(id);
            if (caja == null) return NotFound();
            return View(caja);
        }
    }
}
