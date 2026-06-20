using LocalManager.Models;
using LocalManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly CategoriaService _categoriaService;

        public CategoriasController(CategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public IActionResult Index()
        {
            return View(_categoriaService.GetAll());
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _categoriaService.Add(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        public IActionResult Edit(int id)
        {
            var categoria = _categoriaService.GetById(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        [HttpPost]
        public IActionResult Edit(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _categoriaService.Update(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        public IActionResult Delete(int id)
        {
            var categoria = _categoriaService.GetById(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoriaService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
