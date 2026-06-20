using LocalManager.Models;
using LocalManager.Models.ViewModels;
using LocalManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LocalManager.Controllers
{
    public class VentasController : Controller
    {
        private readonly VentaService _ventaService;
        private readonly ProductoService _productoService;
        private readonly ClienteService _clienteService;
        private readonly CajaService _cajaService;

        public VentasController(VentaService ventaService, ProductoService productoService,
                                ClienteService clienteService, CajaService cajaService)
        {
            _ventaService = ventaService;
            _productoService = productoService;
            _clienteService = clienteService;
            _cajaService = cajaService;
        }

        public IActionResult Index()
        {
            return View(_ventaService.GetAll());
        }

        public IActionResult Create()
        {
            var vm = new VentaViewModel
            {
                Productos = _productoService.GetAll().Where(p => p.Activo && p.Stock > 0)
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Nombre} - ${p.Precio:F2} (Stock: {p.Stock})" }).ToList(),
                Clientes = _clienteService.GetAll()
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre }).ToList(),
                CajasAbiertas = _cajaService.GetAbiertas()
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = $"Caja #{c.Id} - {c.FechaApertura:dd/MM/yyyy HH:mm}" }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
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

            if (_ventaService.Registrar(venta))
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "No hay stock suficiente para uno o más productos.");
            return View(vm);
        }

        public IActionResult Details(int id)
        {
            var venta = _ventaService.GetById(id);
            if (venta == null) return NotFound();
            return View(venta);
        }
    }
}
