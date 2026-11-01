using System.Collections.Concurrent;

namespace ProductCatalog.Api.Services;

public class InMemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, (object Value, DateTime Expiry)> _store = new();

    public T? Get<T>(string key)
    {
        if (_store.TryGetValue(key, out var entry))
        {
            if (entry.Expiry > DateTime.UtcNow) return (T)entry.Value;
            _store.TryRemove(key, out _);
        }
        return default;
    }

    public void Set<T>(string key, T value, TimeSpan ttl) =>
        _store[key] = (value!, DateTime.UtcNow.Add(ttl));

    public void Remove(string key) => _store.TryRemove(key, out _);
}