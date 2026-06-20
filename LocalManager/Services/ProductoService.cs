using LocalManager.Models;

namespace LocalManager.Services
{
    public class ProductoService
    {
        private readonly JsonDatabaseService _db;
        private readonly CategoriaService _catService;

        public ProductoService(JsonDatabaseService db, CategoriaService catService)
        {
            _db = db;
            _catService = catService;
        }

        public List<Producto> GetAll()
        {
            var productos = _db.GetAll<Producto>();
            var categorias = _catService.GetAll().ToDictionary(c => c.Id, c => c.Nombre);
            foreach (var p in productos)
                if (categorias.ContainsKey(p.CategoriaId))
                    p.CategoriaNombre = categorias[p.CategoriaId];
            return productos;
        }

        public Producto? GetById(int id) => GetAll().FirstOrDefault(p => p.Id == id);
        public void Add(Producto p) => _db.Add(p);
        public void Update(Producto p) => _db.Update(p);
        public void Delete(int id) => _db.Delete<Producto>(id);

        public bool DescontarStock(int productoId, int cantidad)
        {
            var p = GetById(productoId);
            if (p == null || p.Stock < cantidad) return false;
            p.Stock -= cantidad;
            Update(p);
            return true;
        }
    }
}
