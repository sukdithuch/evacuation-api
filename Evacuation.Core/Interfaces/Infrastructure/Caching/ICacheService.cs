namespace Evacuation.Core.Interfaces.Infrastructure.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
        Task<HashSet<string>> GetAllKeysAsync();
        Task ClearAllAsync();
    }
}
