using LocalManager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Presentation.Controllers
{
    /// <summary>
    /// CAPA: Presentation — depende de Application (múltiples IService).
    /// ADR-03: Arquitectura en Capas.
    /// </summary>
    public class ReportesController : Controller
    {
        private readonly IVentaService _ventaService;
        private readonly IProductoService _productoService;
        private readonly ICajaService _cajaService;

        public ReportesController(IVentaService ventaService, IProductoService productoService, ICajaService cajaService)
        {
            _ventaService = ventaService;
            _productoService = productoService;
            _cajaService = cajaService;
        }

        public IActionResult Index()
        {
            ViewBag.VentasHoy = _ventaService.ObtenerPorFecha(DateTime.Today);
            ViewBag.VentasMes = _ventaService.ObtenerTodas()
                .Where(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year)
                .GroupBy(v => v.Fecha.Date)
                .OrderBy(g => g.Key)
                .Select(g => new Dictionary<string, object>
                {
                    { "Dia", g.Key },
                    { "Cantidad", g.Count() },
                    { "Monto", g.Sum(v => v.Total) }
                })
                .ToList();
            ViewBag.ProductosBajosStock = _productoService.ObtenerTodos().Where(p => p.Stock < 5).ToList();
            ViewBag.Cajas = _cajaService.ObtenerTodas();
            return View();
        }
    }
}
