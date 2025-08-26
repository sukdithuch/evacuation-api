using Evacuation.Core.Interfaces.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Evacuation.Infrastructure.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private const string AllKeys = "ALL_KEYS";

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cached = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(cached)) return default;
            return JsonSerializer.Deserialize<T>(cached);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = new DistributedCacheEntryOptions();

            if (expiry.HasValue)
                options.SetSlidingExpiration(expiry.Value);
            else
                options.SetSlidingExpiration(TimeSpan.FromDays(1));

            var serialized = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serialized);
            await AddKeyToIndexAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        public async Task<HashSet<string>> GetAllKeysAsync()
        {
            return await GetAsync<HashSet<string>>(AllKeys) ?? new HashSet<string>();
        }

        public async Task ClearAllAsync()
        {
            var keys = await GetAllKeysAsync();
            foreach(var key in keys)
            {
                await _distributedCache.RemoveAsync(key);
            }

            await _distributedCache.RemoveAsync(AllKeys);
        }

        private async Task AddKeyToIndexAsync(string key)
        {
            var keys = await GetAllKeysAsync();
            keys.Add(key);
            var serialized = JsonSerializer.Serialize(keys);
            await _distributedCache.SetStringAsync(AllKeys, serialized);

            //if (allKeyValue?.Any() == false)
            //{
            //    var serialized = JsonSerializer.Serialize(key);
            //    await _distributedCache.SetStringAsync(allKey, serialized);
            //}
            //else
            //{
            //    // ต้องเช็คซ้ำเพิ่ม
            //    allKeyValue += "," + allKeyValue;
            //    var serialized = JsonSerializer.Serialize(allKeyValue);
            //    await _distributedCache.SetStringAsync(allKey, serialized);
            //}
        }        
    }
}
