using LocalManager.Domain.Entities;
using LocalManager.Domain.Interfaces.Repositories;

namespace LocalManager.Application.Services
{
    /// <summary>
    /// Implementación del servicio de caja.
    /// CAPA: Application — orquesta las operaciones usando repositorios de Domain.
    /// </summary>
    public class CajaService : ICajaService
    {
        private readonly ICajaRepository _repository;

        public CajaService(ICajaRepository repository)
        {
            _repository = repository;
        }

        public List<Caja> ObtenerTodas() => _repository.ObtenerTodas();
        public Caja? ObtenerPorId(int id) => _repository.ObtenerPorId(id);
        public List<Caja> ObtenerAbiertas() => _repository.ObtenerAbiertas();

        public void Abrir(Caja caja)
        {
            caja.Estado = "Abierta";
            caja.FechaApertura = DateTime.Now;
            _repository.Agregar(caja);
        }

        public void Cerrar(int id, decimal montoCierre)
        {
            var caja = _repository.ObtenerPorId(id);
            if (caja == null) return;
            caja.FechaCierre = DateTime.Now;
            caja.MontoCierre = montoCierre;
            caja.Estado = "Cerrada";
            _repository.Actualizar(caja);
        }
    }
}
