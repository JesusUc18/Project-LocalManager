namespace LocalManager.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int? ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();
        public decimal Total => Detalles.Sum(d => d.Subtotal);
        public string MetodoPago { get; set; } = "Efectivo";
        public int CajaId { get; set; }
    }
}
