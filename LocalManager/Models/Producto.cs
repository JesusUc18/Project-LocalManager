namespace LocalManager.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int CategoriaId { get; set; }
        public string? CategoriaNombre { get; set; }
        public string? CodigoBarras { get; set; }
        public bool Activo { get; set; } = true;
    }
}
