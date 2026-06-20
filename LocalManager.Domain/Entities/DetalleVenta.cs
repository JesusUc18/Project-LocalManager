using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalManager.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio: Línea de detalle de una venta.
    /// VISTA DE LÓGICA (ADR-02): Módulo de Ventas → Líneas de detalle por venta
    /// CAPA: Domain (centro de Clean Architecture) — no depende de ningún proyecto externo
    /// </summary>
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VentaId { get; set; }

        public Venta? Venta { get; set; }

        [Required]
        public int ProductoId { get; set; }

        public Producto? Producto { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [NotMapped]
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
