using System.Text.Json;
using System.Text.Json.Serialization;
using LocalManager.Domain.Entities;

namespace LocalManager.Infrastructure.Data
{
    /// <summary>
    /// Contexto de datos temporal basado en archivos JSON.
    /// VISTA DE DESARROLLO (ADR-02): Capa Infrastructure → Implementación de persistencia
    /// VISTA DE DESPLIEGUE (ADR-02): Persistencia temporal en archivos JSON (reemplazar por SQL Server)
    /// 
    /// Simula el comportamiento de un DbContext de EF Core con Set&lt;T&gt;(), Add(), SaveChanges().
    /// CAPA: Infrastructure — implementa la persistencia, depende de Domain (entidades).
    /// </summary>
    public class JsonDbContext
    {
        private readonly string _dataPath;
        private readonly object _lock = new();
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly Dictionary<Type, object> _sets = new();

        public JsonDbContext(string dataPath = "Data")
        {
            _dataPath = dataPath;
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), _dataPath);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public List<T> Set<T>() where T : class
        {
            var type = typeof(T);
            if (!_sets.ContainsKey(type))
                _sets[type] = LoadFromDisk<T>();
            return (List<T>)_sets[type];
        }

        public int SaveChanges()
        {
            lock (_lock)
            {
                int changes = 0;
                foreach (var kvp in _sets)
                {
                    var type = kvp.Key;
                    var method = typeof(JsonDbContext).GetMethod(nameof(SaveSet), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var genericMethod = method?.MakeGenericMethod(type);
                    var result = genericMethod?.Invoke(this, new[] { kvp.Value });
                    if (result is int count) changes += count;
                }
                return changes;
            }
        }

        public void Add<T>(T entity) where T : class
        {
            var set = Set<T>();
            var idProp = typeof(T).GetProperty("Id");
            if (idProp != null)
            {
                int maxId = set.Any() ? set.Max(x => (int)(idProp.GetValue(x) ?? 0)) : 0;
                idProp.SetValue(entity, maxId + 1);
            }
            set.Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            var set = Set<T>();
            var idProp = typeof(T).GetProperty("Id");
            if (idProp == null) return;
            int id = (int)(idProp.GetValue(entity) ?? 0);
            var index = set.FindIndex(x => (int)(idProp.GetValue(x) ?? 0) == id);
            if (index >= 0) set[index] = entity;
        }

        public void Remove<T>(int id) where T : class
        {
            var set = Set<T>();
            var idProp = typeof(T).GetProperty("Id");
            if (idProp == null) return;
            set.RemoveAll(x => (int)(idProp.GetValue(x) ?? 0) == id);
        }

        public T? Find<T>(int id) where T : class
        {
            var set = Set<T>();
            var idProp = typeof(T).GetProperty("Id");
            if (idProp == null) return null;
            return set.FirstOrDefault(x => (int)(idProp.GetValue(x) ?? 0) == id);
        }

        private List<T> LoadFromDisk<T>() where T : class
        {
            var path = GetFilePath<T>();
            if (!File.Exists(path)) return new List<T>();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions) ?? new List<T>();
        }

        private int SaveSet<T>(object data) where T : class
        {
            var path = GetFilePath<T>();
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(path, json);
            return ((List<T>)data).Count;
        }

        private string GetFilePath<T>() => Path.Combine(
            Directory.GetCurrentDirectory(), _dataPath, $"{typeof(T).Name.ToLower()}s.json");
    }
}
