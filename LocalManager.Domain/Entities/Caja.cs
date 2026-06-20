using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalManager.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio: Turno de caja.
    /// VISTA DE LÓGICA (ADR-02): Módulo de Caja → Control de turnos y montos
    /// CAPA: Domain (centro de Clean Architecture) — no depende de ningún proyecto externo
    /// </summary>
    public class Caja
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaApertura { get; set; } = DateTime.Now;

        public DateTime? FechaCierre { get; set; }

        [Required]
        [Range(0, 999999.99)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoInicial { get; set; }

        [Range(0, 999999.99)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MontoCierre { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Abierta";

        [StringLength(100)]
        public string? Responsable { get; set; }

        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();

        [NotMapped]
        public decimal TotalVentas => Ventas.Sum(v => v.Total);
    }
}
