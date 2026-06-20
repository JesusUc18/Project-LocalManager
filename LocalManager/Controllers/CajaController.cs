using LocalManager.Models;
using LocalManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Controllers
{
    public class CajaController : Controller
    {
        private readonly CajaService _cajaService;

        public CajaController(CajaService cajaService)
        {
            _cajaService = cajaService;
        }

        public IActionResult Index()
        {
            return View(_cajaService.GetAll());
        }

        public IActionResult Abrir() => View();

        [HttpPost]
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
            var caja = _cajaService.GetById(id);
            if (caja == null) return NotFound();
            return View(caja);
        }

        [HttpPost]
        public IActionResult Cerrar(int id, decimal montoCierre)
        {
            _cajaService.Cerrar(id, montoCierre);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var caja = _cajaService.GetById(id);
            if (caja == null) return NotFound();
            return View(caja);
        }
    }
}
