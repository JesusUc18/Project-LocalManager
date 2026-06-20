using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Presentation.Controllers
{
    /// <summary>
    /// CAPA: Presentation — depende de Application (ICategoriaService).
    /// ADR-03: Arquitectura en Capas.
    /// </summary>
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public IActionResult Index() => View(_categoriaService.ObtenerTodas());
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _categoriaService.Crear(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        public IActionResult Edit(int id)
        {
            var categoria = _categoriaService.ObtenerPorId(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _categoriaService.Actualizar(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        public IActionResult Delete(int id)
        {
            var categoria = _categoriaService.ObtenerPorId(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoriaService.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
