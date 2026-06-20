using LocalManager.Domain.Entities;

namespace LocalManager.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato de repositorio para Cajas.
    /// CAPA: Domain — define el contrato, Infrastructure lo implementa.
    /// </summary>
    public interface ICajaRepository
    {
        List<Caja> ObtenerTodas();
        Caja? ObtenerPorId(int id);
        List<Caja> ObtenerAbiertas();
        void Agregar(Caja caja);
        void Actualizar(Caja caja);
    }
}
