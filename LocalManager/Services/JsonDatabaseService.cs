using System.Text.Json;

namespace LocalManager.Services
{
    public class JsonDatabaseService
    {
        private readonly string _dataPath;
        private readonly object _lock = new();

        public JsonDatabaseService(IConfiguration configuration)
        {
            _dataPath = configuration.GetValue<string>("JsonDatabase:DataPath") ?? "Data";
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), _dataPath);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
        }

        private string GetFilePath<T>() => Path.Combine(
            Directory.GetCurrentDirectory(), _dataPath, $"{typeof(T).Name.ToLower()}s.json");

        public List<T> GetAll<T>() where T : class
        {
            var path = GetFilePath<T>();
            if (!File.Exists(path)) return new List<T>();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        public T? GetById<T>(int id) where T : class
        {
            var items = GetAll<T>();
            var prop = typeof(T).GetProperty("Id");
            return items.FirstOrDefault(x => (int)(prop?.GetValue(x) ?? 0) == id);
        }

        public void Add<T>(T item) where T : class
        {
            lock (_lock)
            {
                var items = GetAll<T>();
                var prop = typeof(T).GetProperty("Id");
                int maxId = items.Any() ? items.Max(x => (int)(prop?.GetValue(x) ?? 0)) : 0;
                prop?.SetValue(item, maxId + 1);
                items.Add(item);
                SaveAll(items);
            }
        }

        public void Update<T>(T item) where T : class
        {
            lock (_lock)
            {
                var items = GetAll<T>();
                var prop = typeof(T).GetProperty("Id");
                int id = (int)(prop?.GetValue(item) ?? 0);
                var index = items.FindIndex(x => (int)(prop?.GetValue(x) ?? 0) == id);
                if (index >= 0)
                {
                    items[index] = item;
                    SaveAll(items);
                }
            }
        }

        public void Delete<T>(int id) where T : class
        {
            lock (_lock)
            {
                var items = GetAll<T>();
                var prop = typeof(T).GetProperty("Id");
                items.RemoveAll(x => (int)(prop?.GetValue(x) ?? 0) == id);
                SaveAll(items);
            }
        }

        private void SaveAll<T>(List<T> items) where T : class
        {
            var path = GetFilePath<T>();
            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }
}
