using Evacuation.Core.Interfaces.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Evacuation.Infrastructure.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var cached = await _distributedCache.GetStringAsync(key);
                if (string.IsNullOrEmpty(cached)) return default;
                return JsonSerializer.Deserialize<T>(cached);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions();

                if (expiry.HasValue)
                    options.SetSlidingExpiration(expiry.Value);

                var serialized = JsonSerializer.Serialize(value);
                await _distributedCache.SetStringAsync(key, serialized);
                await KeepAllKey(key);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
            }
            catch (Exception ex)
            {

            }
        }

        private async Task KeepAllKey(string key)
        {
            string allKey = "ALLKEY";
            var allKeyValue = await GetAsync<string>(allKey);
            if (string.IsNullOrEmpty(allKeyValue))
            {
                var serialized = JsonSerializer.Serialize(key);
                await _distributedCache.SetStringAsync(allKey, serialized);
            }
            else
            {
                // ต้องเช็คซ้ำเพิ่ม
                allKeyValue += "," + allKeyValue;
                var serialized = JsonSerializer.Serialize(allKeyValue);
                await _distributedCache.SetStringAsync(allKey, serialized);
            }
        }
    }
}
