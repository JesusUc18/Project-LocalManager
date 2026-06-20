using Microsoft.AspNetCore.Mvc.Rendering;

namespace LocalManager.Models.ViewModels
{
    public class VentaViewModel
    {
        public Venta Venta { get; set; } = new Venta();
        public List<SelectListItem> Productos { get; set; } = new();
        public List<SelectListItem> Clientes { get; set; } = new();
        public List<SelectListItem> CajasAbiertas { get; set; } = new();
        public List<SelectListItem> MetodosPago { get; set; } = new()
        {
            new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
            new SelectListItem { Value = "Tarjeta", Text = "Tarjeta" },
            new SelectListItem { Value = "Transferencia", Text = "Transferencia" }
        };
    }
}
