using LocalManager.Models;
using LocalManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LocalManager.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoService _productoService;
        private readonly CategoriaService _categoriaService;

        public ProductosController(ProductoService productoService, CategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        public IActionResult Index()
        {
            var productos = _productoService.GetAll();
            return View(productos);
        }

        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(_categoriaService.GetAll(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _productoService.Add(producto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categorias = new SelectList(_categoriaService.GetAll(), "Id", "Nombre");
            return View(producto);
        }

        public IActionResult Edit(int id)
        {
            var producto = _productoService.GetById(id);
            if (producto == null) return NotFound();
            ViewBag.Categorias = new SelectList(_categoriaService.GetAll(), "Id", "Nombre");
            return View(producto);
        }

        [HttpPost]
        public IActionResult Edit(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _productoService.Update(producto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categorias = new SelectList(_categoriaService.GetAll(), "Id", "Nombre");
            return View(producto);
        }

        public IActionResult Delete(int id)
        {
            var producto = _productoService.GetById(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _productoService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var producto = _productoService.GetById(id);
            if (producto == null) return NotFound();
            return View(producto);
        }
    }
}
