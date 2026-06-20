namespace LocalManager.Models
{
    public class Caja
    {
        public int Id { get; set; }
        public DateTime FechaApertura { get; set; } = DateTime.Now;
        public DateTime? FechaCierre { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal? MontoCierre { get; set; }
        public string Estado { get; set; } = "Abierta"; // Abierta, Cerrada
        public string? Responsable { get; set; }
        public List<Venta> Ventas { get; set; } = new();
        public decimal TotalVentas => Ventas.Sum(v => v.Total);
    }
}
