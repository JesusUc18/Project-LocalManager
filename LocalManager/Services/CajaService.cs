using LocalManager.Models;

namespace LocalManager.Services
{
    public class CajaService
    {
        private readonly JsonDatabaseService _db;
        private readonly VentaService _ventaService;

        public CajaService(JsonDatabaseService db, VentaService ventaService)
        {
            _db = db;
            _ventaService = ventaService;
        }

        public List<Caja> GetAll()
        {
            var cajas = _db.GetAll<Caja>();
            var ventas = _ventaService.GetAll();
            foreach (var caja in cajas)
                caja.Ventas = ventas.Where(v => v.CajaId == caja.Id).ToList();
            return cajas;
        }

        public Caja? GetById(int id) => GetAll().FirstOrDefault(c => c.Id == id);
        public List<Caja> GetAbiertas() => GetAll().Where(c => c.Estado == "Abierta").ToList();

        public void Abrir(Caja caja)
        {
            caja.Estado = "Abierta";
            caja.FechaApertura = DateTime.Now;
            _db.Add(caja);
        }

        public void Cerrar(int id, decimal montoCierre)
        {
            var caja = GetById(id);
            if (caja == null) return;
            caja.FechaCierre = DateTime.Now;
            caja.MontoCierre = montoCierre;
            caja.Estado = "Cerrada";
            _db.Update(caja);
        }
    }
}
