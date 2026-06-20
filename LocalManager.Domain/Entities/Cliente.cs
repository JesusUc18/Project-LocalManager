using System.ComponentModel.DataAnnotations;

namespace LocalManager.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio: Cliente registrado en el sistema.
    /// VISTA DE LÓGICA (ADR-02): Módulo de Clientes → Administración de clientes
    /// CAPA: Domain (centro de Clean Architecture) — no depende de ningún proyecto externo
    /// </summary>
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(300)]
        public string? Direccion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
