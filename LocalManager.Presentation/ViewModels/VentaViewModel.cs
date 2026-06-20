using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using LocalManager.Domain.Entities;

namespace LocalManager.Presentation.ViewModels
{
    /// <summary>
    /// ViewModel para la vista de creación de ventas.
    /// VISTA DE LÓGICA (ADR-02): Módulo de Ventas → Formulario de registro
    /// CAPA: Presentation — DTO para la capa de presentación (puede usar ASP.NET Core MVC).
    /// ADR-03: Este ViewModel vive en Presentation, NO en Application, porque usa SelectListItem.
    /// </summary>
    public class VentaViewModel
    {
        public Venta Venta { get; set; } = new Venta();

        [Display(Name = "Productos disponibles")]
        public List<SelectListItem> Productos { get; set; } = new();

        [Display(Name = "Clientes registrados")]
        public List<SelectListItem> Clientes { get; set; } = new();

        [Display(Name = "Cajas abiertas")]
        public List<SelectListItem> CajasAbiertas { get; set; } = new();

        public List<SelectListItem> MetodosPago { get; set; } = new()
        {
            new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
            new SelectListItem { Value = "Tarjeta", Text = "Tarjeta de crédito/débito" },
            new SelectListItem { Value = "Transferencia", Text = "Transferencia bancaria" }
        };
    }
}
