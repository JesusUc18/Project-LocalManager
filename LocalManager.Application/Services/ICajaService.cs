using LocalManager.Domain.Entities;

namespace LocalManager.Application.Services
{
    /// <summary>
    /// VISTA DE LÓGICA (ADR-02): Módulo de Caja → Control de turnos
    /// VISTA DE PROCESOS (ADR-02): Application → Domain → Repositorio → Infrastructure
    /// CAPA: Application — contiene las reglas de negocio, depende solo de Domain.
    /// </summary>
    public interface ICajaService
    {
        List<Caja> ObtenerTodas();
        Caja? ObtenerPorId(int id);
        List<Caja> ObtenerAbiertas();
        void Abrir(Caja caja);
        void Cerrar(int id, decimal montoCierre);
    }
}
