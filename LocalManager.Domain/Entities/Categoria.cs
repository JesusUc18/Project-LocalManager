using System.ComponentModel.DataAnnotations;

namespace LocalManager.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio: Categoría de productos.
    /// VISTA DE LÓGICA (ADR-02): Módulo de Inventario → Clasificación de productos
    /// CAPA: Domain (centro de Clean Architecture) — no depende de ningún proyecto externo
    /// </summary>
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Descripcion { get; set; }
    }
}
