using LocalManager.Models;

namespace LocalManager.Services
{
    public class ClienteService
    {
        private readonly JsonDatabaseService _db;
        public ClienteService(JsonDatabaseService db) => _db = db;

        public List<Cliente> GetAll() => _db.GetAll<Cliente>();
        public Cliente? GetById(int id) => _db.GetById<Cliente>(id);
        public void Add(Cliente c) => _db.Add(c);
        public void Update(Cliente c) => _db.Update(c);
        public void Delete(int id) => _db.Delete<Cliente>(id);
    }
}
