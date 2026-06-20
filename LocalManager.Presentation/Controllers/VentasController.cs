using LocalManager.Application.Services;
using LocalManager.Presentation.ViewModels;
using LocalManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LocalManager.Presentation.Controllers
{
    /// <summary>
    /// VISTA DE LÓGICA (ADR-02): Módulo de Ventas → Registro de ventas con múltiples productos
    /// VISTA DE PROCESOS (ADR-02): HTTP POST → Controller → IVentaService.Registrar() → Transacción atómica
    /// CAPA: Presentation — depende de Application, NO de Infrastructure.
    /// ADR-03: Arquitectura en Capas.
    /// </summary>
    public class VentasController : Controller
    {
        private readonly IVentaService _ventaService;
        private readonly IProductoService _productoService;
        private readonly IClienteService _clienteService;
        private readonly ICajaService _cajaService;

        public VentasController(IVentaService ventaService, IProductoService productoService,
                                IClienteService clienteService, ICajaService cajaService)
        {
            _ventaService = ventaService;
            _productoService = productoService;
            _clienteService = clienteService;
            _cajaService = cajaService;
        }

        public IActionResult Index() => View(_ventaService.ObtenerTodas());

        public IActionResult Create()
        {
            var vm = new VentaViewModel
            {
                Productos = _productoService.ObtenerTodos().Where(p => p.Activo && p.Stock > 0)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Nombre} - ${p.Precio:F2} (Stock: {p.Stock})" }).ToList(),
                Clientes = _clienteService.ObtenerTodos()
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre }).ToList(),
                CajasAbiertas = _cajaService.ObtenerAbiertas()
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"Caja #{c.Id} - {c.FechaApertura:dd/MM/yyyy HH:mm}" }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VentaViewModel vm, int[] productoIds, int[] cantidades)
        {
            var venta = vm.Venta;
            venta.Detalles = new List<DetalleVenta>();

            for (int i = 0; i < productoIds.Length; i++)
            {
                if (cantidades[i] > 0)
                {
                    venta.Detalles.Add(new DetalleVenta
                    {
                        ProductoId = productoIds[i],
                        Cantidad = cantidades[i]
                    });
                }
            }

            if (!venta.Detalles.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto a la venta.");
                return View(vm);
            }

            var (exito, mensaje) = _ventaService.Registrar(venta);
            if (exito)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", mensaje);
            return View(vm);
        }

        public IActionResult Details(int id)
        {
            var venta = _ventaService.ObtenerPorId(id);
            if (venta == null) return NotFound();
            return View(venta);
        }
    }
}
