using LocalManager.Domain.Entities;

namespace LocalManager.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato de repositorio para Productos.
    /// CAPA: Domain — define el contrato, Infrastructure lo implementa.
    /// </summary>
    public interface IProductoRepository
    {
        List<Producto> ObtenerTodos();
        Producto? ObtenerPorId(int id);
        void Agregar(Producto producto);
        void Actualizar(Producto producto);
        void Eliminar(int id);
    }
}
