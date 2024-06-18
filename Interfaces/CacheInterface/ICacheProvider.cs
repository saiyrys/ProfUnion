namespace Profunion.Interfaces.CacheInterface
{
    public interface ICacheProvider
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan expiration);
        void Remove<T>(string key);
        bool Exists(string key);
    }
}
