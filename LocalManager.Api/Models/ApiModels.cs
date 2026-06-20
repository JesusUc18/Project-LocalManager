namespace LocalManager.Api.Models
{
    /// <summary>
    /// DTO para respuestas genéricas de la API.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public T? Datos { get; set; }
    }

    /// <summary>
    /// DTO para crear una nueva venta.
    /// </summary>
    public class CrearVentaRequest
    {
        public int? ClienteId { get; set; }
        public int CajaId { get; set; }
        public string MetodoPago { get; set; } = "Efectivo";
        public List<DetalleVentaRequest> Detalles { get; set; } = new();
    }

    public class DetalleVentaRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// DTO para abrir una caja.
    /// </summary>
    public class AbrirCajaRequest
    {
        public decimal MontoInicial { get; set; }
        public string? Responsable { get; set; }
    }

    /// <summary>
    /// DTO para cerrar una caja.
    /// </summary>
    public class CerrarCajaRequest
    {
        public decimal MontoCierre { get; set; }
    }
}
