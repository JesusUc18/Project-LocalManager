using LocalManager.Domain.Entities;

namespace LocalManager.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato de repositorio para Clientes.
    /// CAPA: Domain — define el contrato, Infrastructure lo implementa.
    /// </summary>
    public interface IClienteRepository
    {
        List<Cliente> ObtenerTodos();
        Cliente? ObtenerPorId(int id);
        void Agregar(Cliente cliente);
        void Actualizar(Cliente cliente);
        void Eliminar(int id);
    }
}
