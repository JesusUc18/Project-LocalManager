using LocalManager.Models;

namespace LocalManager.Services
{
    public class CategoriaService
    {
        private readonly JsonDatabaseService _db;
        public CategoriaService(JsonDatabaseService db) => _db = db;

        public List<Categoria> GetAll() => _db.GetAll<Categoria>();
        public Categoria? GetById(int id) => _db.GetById<Categoria>(id);
        public void Add(Categoria c) => _db.Add(c);
        public void Update(Categoria c) => _db.Update(c);
        public void Delete(int id) => _db.Delete<Categoria>(id);
    }
}
