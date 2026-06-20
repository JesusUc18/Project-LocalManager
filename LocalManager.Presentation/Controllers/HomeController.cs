using LocalManager.Application.Services;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LocalManager.Presentation.Controllers
{
    /// <summary>
    /// VISTA DE LÓGICA (ADR-02): Dashboard → Resumen del negocio
    /// VISTA DE PROCESOS (ADR-02): HTTP → Controller → IService (Application) → IRepository (Domain) → Infrastructure
    /// CAPA: Presentation — depende de Application (IService), NO de Infrastructure directamente.
    /// ADR-03: Arquitectura en Capas — Controller solo conoce interfaces de Application.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly IVentaService _ventaService;
        private readonly ICajaService _cajaService;

        public HomeController(IProductoService productoService, IVentaService ventaService, ICajaService cajaService)
        {
            _productoService = productoService;
            _ventaService = ventaService;
            _cajaService = cajaService;
        }

        public IActionResult Index()
        {
            var productos = _productoService.ObtenerTodos();
            var ventas = _ventaService.ObtenerTodas();
            var cajasAbiertas = _cajaService.ObtenerAbiertas();

            ViewBag.TotalProductos = productos.Count;
            ViewBag.ProductosBajosStock = productos.Count(p => p.Stock < 5);
            ViewBag.VentasHoy = ventas.Count(v => v.Fecha.Date == DateTime.Today);
            ViewBag.TotalVentasHoy = ventas.Where(v => v.Fecha.Date == DateTime.Today).Sum(v => v.Total);
            ViewBag.CajasAbiertas = cajasAbiertas.Count;

            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
