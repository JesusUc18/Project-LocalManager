using LocalManager.Domain.Entities;
using LocalManager.Domain.Interfaces.Repositories;

namespace LocalManager.Application.Services
{
    /// <summary>
    /// Implementación del servicio de productos.
    /// CAPA: Application — orquesta las operaciones usando repositorios de Domain.
    /// </summary>
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repository;

        public ProductoService(IProductoRepository repository)
        {
            _repository = repository;
        }

        public List<Producto> ObtenerTodos() => _repository.ObtenerTodos();
        public Producto? ObtenerPorId(int id) => _repository.ObtenerPorId(id);
        public void Crear(Producto producto) => _repository.Agregar(producto);
        public void Actualizar(Producto producto) => _repository.Actualizar(producto);
        public void Eliminar(int id) => _repository.Eliminar(id);

        public bool DescontarStock(int productoId, int cantidad)
        {
            var producto = _repository.ObtenerPorId(productoId);
            if (producto == null || producto.Stock < cantidad)
                return false;
            producto.Stock -= cantidad;
            _repository.Actualizar(producto);
            return true;
        }

        public void RevertirStock(int productoId, int cantidad)
        {
            var producto = _repository.ObtenerPorId(productoId);
            if (producto != null)
            {
                producto.Stock += cantidad;
                _repository.Actualizar(producto);
            }
        }
    }
}
