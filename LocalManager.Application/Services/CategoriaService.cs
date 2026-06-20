using LocalManager.Domain.Entities;
using LocalManager.Domain.Interfaces.Repositories;

namespace LocalManager.Application.Services
{
    /// <summary>
    /// Implementación del servicio de categorías.
    /// CAPA: Application — orquesta las operaciones usando repositorios de Domain.
    /// </summary>
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repository;

        public CategoriaService(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        public List<Categoria> ObtenerTodas() => _repository.ObtenerTodas();
        public Categoria? ObtenerPorId(int id) => _repository.ObtenerPorId(id);

        public void Crear(Categoria categoria) => _repository.Agregar(categoria);
        public void Actualizar(Categoria categoria) => _repository.Actualizar(categoria);
        public void Eliminar(int id) => _repository.Eliminar(id);
    }
}
