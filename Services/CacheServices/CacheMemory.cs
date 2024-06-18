using Microsoft.Extensions.Caching.Memory;
using Profunion.Interfaces.CacheInterface;

namespace Profunion.Services.CacheServices
{
    public class CacheMemory : ICacheProvider
    {
        private MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }
        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(expiration);

            _cache.Set(key, value, cacheOptions);
        }
        
        public void Remove<T>(string key)
        {
            _cache.Remove(key);
        }
        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }



    }
}
