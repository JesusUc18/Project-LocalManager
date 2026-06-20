using LocalManager.Domain.Entities;

namespace LocalManager.Application.Services
{
    /// <summary>
    /// VISTA DE LÓGICA (ADR-02): Módulo de Inventario → Gestión de productos y stock
    /// VISTA DE PROCESOS (ADR-02): Application → Domain → Repositorio → Infrastructure
    /// CAPA: Application — contiene las reglas de negocio, depende solo de Domain.
    /// </summary>
    public interface IProductoService
    {
        List<Producto> ObtenerTodos();
        Producto? ObtenerPorId(int id);
        void Crear(Producto producto);
        void Actualizar(Producto producto);
        void Eliminar(int id);
        bool DescontarStock(int productoId, int cantidad);
        void RevertirStock(int productoId, int cantidad);
    }
}
