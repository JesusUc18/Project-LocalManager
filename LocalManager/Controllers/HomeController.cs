using LocalManager.Models;
using LocalManager.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LocalManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductoService _productoService;
        private readonly VentaService _ventaService;
        private readonly CajaService _cajaService;

        public HomeController(ProductoService productoService, VentaService ventaService, CajaService cajaService)
        {
            _productoService = productoService;
            _ventaService = ventaService;
            _cajaService = cajaService;
        }

        public IActionResult Index()
        {
            var productos = _productoService.GetAll();
            var ventas = _ventaService.GetAll();
            var cajasAbiertas = _cajaService.GetAbiertas();

            ViewBag.TotalProductos = productos.Count;
            ViewBag.ProductosBajosStock = productos.Count(p => p.Stock < 5);
            ViewBag.VentasHoy = ventas.Count(v => v.Fecha.Date == DateTime.Today);
            ViewBag.TotalVentasHoy = ventas.Where(v => v.Fecha.Date == DateTime.Today).Sum(v => v.Total);
            ViewBag.CajasAbiertas = cajasAbiertas.Count;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
