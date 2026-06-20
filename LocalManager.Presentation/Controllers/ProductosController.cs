using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LocalManager.Presentation.Controllers
{
    /// <summary>
    /// VISTA DE LÓGICA (ADR-02): Módulo de Inventario → CRUD de Productos
    /// VISTA DE PROCESOS (ADR-02): HTTP → Controller → IProductoService (Application)
    /// CAPA: Presentation — depende de Application, NO de Infrastructure.
    /// ADR-03: Arquitectura en Capas.
    /// </summary>
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;

        public ProductosController(IProductoService productoService, ICategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        public IActionResult Index() => View(_productoService.ObtenerTodos());

        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(_categoriaService.ObtenerTodas(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _productoService.Crear(producto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categorias = new SelectList(_categoriaService.ObtenerTodas(), "Id", "Nombre");
            return View(producto);
        }

        public IActionResult Edit(int id)
        {
            var producto = _productoService.ObtenerPorId(id);
            if (producto == null) return NotFound();
            ViewBag.Categorias = new SelectList(_categoriaService.ObtenerTodas(), "Id", "Nombre");
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _productoService.Actualizar(producto);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categorias = new SelectList(_categoriaService.ObtenerTodas(), "Id", "Nombre");
            return View(producto);
        }

        public IActionResult Delete(int id)
        {
            var producto = _productoService.ObtenerPorId(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _productoService.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var producto = _productoService.ObtenerPorId(id);
            if (producto == null) return NotFound();
            return View(producto);
        }
    }
}
