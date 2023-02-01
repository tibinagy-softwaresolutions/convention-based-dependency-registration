using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Text.Json;
using TNArch.DependencyInjection.Convention.Abstractions;
using TNArch.DependencyInjection.Convention.Demo.Logger;

namespace TNArch.DependencyInjection.Convention.Demo.Cache
{
    [Dependency(typeof(ICacheService), DependencyLifeStyle.Singleton)]
    public class CacheService : ICacheService
    {
        private IDistributedCache _cacheService;
        private readonly DistributedCacheConfig _distributedCacheConfig;
        private readonly ILogger<CacheService> _logger;
        private readonly ConcurrentDictionary<string, Task<object>> _localCache;

        public CacheService(IDistributedCache cacheService, DistributedCacheConfig distributedCacheConfig, ILogger<CacheService> logger)
        {
            _cacheService = cacheService;
            _distributedCacheConfig = distributedCacheConfig;
            _logger = logger;
            _localCache = new ConcurrentDictionary<string, Task<object>>();
        }

        public async Task<TValue> GetOrCreate<TValue>(string key, Func<Task<TValue>> valueFactory)
        {
            return (TValue)await _localCache.GetOrAdd(key, async k => await GetOrCreateValue(key, valueFactory));
        }

        public virtual async Task<TValue> GetOrCreateValue<TValue>(string key, Func<Task<TValue>> valueFactory)
        {
            try
            {
                var jsonValue = await _cacheService.GetStringAsync(key).RunWithinTimeout(_distributedCacheConfig.ReadTimeout);

                if (jsonValue != default)
                    return JsonSerializer.Deserialize<TValue>(jsonValue);

                var value = await valueFactory();

                jsonValue = JsonSerializer.Serialize(value);

                await _cacheService.SetStringAsync(key, jsonValue, _distributedCacheConfig).RunWithinTimeout(_distributedCacheConfig.WriteTimeout);

                return value;
            }
            catch (StackExchange.Redis.RedisConnectionException ex)
            {
                _cacheService = new NullDistributedCache();
                _logger.LogWarning($"Redis cache disabled due to connection error: {ex.Message}");

                return await GetOrCreateValue(key, valueFactory);
            }
        }

        public async Task RemoveValue(string key)
        {
            try
            {
                _localCache.TryRemove(key, out _);
                await _cacheService.RemoveAsync(key).RunWithinTimeout(_distributedCacheConfig.WriteTimeout);
            }
            catch (StackExchange.Redis.RedisConnectionException ex)
            {
                _cacheService = new NullDistributedCache();
                _logger.LogWarning($"Redis cache disabled due to connection error: {ex.Message}");
            }
        }
    }
}