using LocalManager.Domain.Entities;
using LocalManager.Domain.Interfaces.Repositories;

namespace LocalManager.Application.Services
{
    /// <summary>
    /// Implementación del servicio de clientes.
    /// CAPA: Application — orquesta las operaciones usando repositorios de Domain.
    /// </summary>
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repository;

        public ClienteService(IClienteRepository repository)
        {
            _repository = repository;
        }

        public List<Cliente> ObtenerTodos() => _repository.ObtenerTodos();
        public Cliente? ObtenerPorId(int id) => _repository.ObtenerPorId(id);
        public void Crear(Cliente cliente) => _repository.Agregar(cliente);
        public void Actualizar(Cliente cliente) => _repository.Actualizar(cliente);
        public void Eliminar(int id) => _repository.Eliminar(id);
    }
}
