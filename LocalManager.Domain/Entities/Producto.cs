using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalManager.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio: Producto del inventario.
    /// VISTA DE LÓGICA (ADR-02): Módulo de Inventario → Control de stock y precios
    /// CAPA: Domain (centro de Clean Architecture) — no depende de ningún proyecto externo
    /// </summary>
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        public Categoria? Categoria { get; set; }

        [StringLength(50)]
        public string? CodigoBarras { get; set; }

        public bool Activo { get; set; } = true;
    }
}
