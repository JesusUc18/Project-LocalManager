using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalManager.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio: Venta.
    /// VISTA DE LÓGICA (ADR-02): Módulo de Ventas → Registro de ventas con múltiples productos
    /// CAPA: Domain (centro de Clean Architecture) — no depende de ningún proyecto externo
    /// </summary>
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        public int? ClienteId { get; set; }

        public Cliente? Cliente { get; set; }

        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

        [NotMapped]
        public decimal Total => Detalles.Sum(d => d.Subtotal);

        [Required]
        [StringLength(50)]
        public string MetodoPago { get; set; } = "Efectivo";

        [Required]
        public int CajaId { get; set; }

        public Caja? Caja { get; set; }
    }
}
