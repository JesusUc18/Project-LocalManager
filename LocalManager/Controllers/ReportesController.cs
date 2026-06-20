using LocalManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Controllers
{
    public class ReportesController : Controller
    {
        private readonly VentaService _ventaService;
        private readonly ProductoService _productoService;
        private readonly CajaService _cajaService;

        public ReportesController(VentaService ventaService, ProductoService productoService, CajaService cajaService)
        {
            _ventaService = ventaService;
            _productoService = productoService;
            _cajaService = cajaService;
        }

        public IActionResult Index()
        {
            var ventas = _ventaService.GetAll();
            var productos = _productoService.GetAll();
            var cajas = _cajaService.GetAll();

            ViewBag.VentasHoy = ventas.Where(v => v.Fecha.Date == DateTime.Today).ToList();
            ViewBag.VentasMes = ventas.Where(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year).ToList();
            ViewBag.ProductosBajosStock = productos.Where(p => p.Stock < 5).ToList();
            ViewBag.Cajas = cajas;

            return View();
        }
    }
}
