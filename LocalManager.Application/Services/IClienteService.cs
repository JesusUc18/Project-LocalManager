using LocalManager.Domain.Entities;

namespace LocalManager.Application.Services
{
    /// <summary>
    /// VISTA DE LÓGICA (ADR-02): Módulo de Clientes → Gestión de clientes
    /// VISTA DE PROCESOS (ADR-02): Application → Domain → Repositorio → Infrastructure
    /// CAPA: Application — contiene las reglas de negocio, depende solo de Domain.
    /// </summary>
    public interface IClienteService
    {
        List<Cliente> ObtenerTodos();
        Cliente? ObtenerPorId(int id);
        void Crear(Cliente cliente);
        void Actualizar(Cliente cliente);
        void Eliminar(int id);
    }
}
