using LocalManager.Domain.Entities;

namespace LocalManager.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato de repositorio para Categorías.
    /// CAPA: Domain — define el contrato, Infrastructure lo implementa.
    /// Inversión de dependencias: Domain no conoce Infrastructure.
    /// </summary>
    public interface ICategoriaRepository
    {
        List<Categoria> ObtenerTodas();
        Categoria? ObtenerPorId(int id);
        void Agregar(Categoria categoria);
        void Actualizar(Categoria categoria);
        void Eliminar(int id);
    }
}
