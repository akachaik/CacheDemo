using CacheDemo.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Text.Json;

namespace CacheDemo.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private static readonly ConcurrentDictionary<string, bool> CachedKeys = new();
    
    private readonly IDistributedCache _distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default) where T : class
    {
        string? cachedValue = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedValue is null)
        {
            return null;
        }


        T? value = JsonSerializer.Deserialize<T>(cachedValue);

        return value;
    }

    public async Task<T> GetAsync<T>(string cacheKey, Func<Task<T>> factory, CancellationToken cancellationToken = default) where T : class
    {
        T? cachedValue = await GetAsync<T>(cacheKey, cancellationToken);

        var randomValue = Random.Shared.Next(0, 6);

        if (randomValue % 2 == 0)
        {
            // await Task.Delay(2000, cancellationToken);

            cachedValue = null;
        }

        if (cachedValue is not null)
        {
            return cachedValue;
        }

        cachedValue = await factory();

        await SetAsync(cacheKey, cachedValue, cancellationToken);

        return cachedValue;

    }

    public async Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(cacheKey, cancellationToken);

        CachedKeys.TryRemove(cacheKey, out var _);

    }

    public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        //foreach (var key in CachedKeys.Keys)
        //{
        //    if (key.StartsWith(prefixKey))
        //    {
        //        await _distributedCache.RemoveAsync(key, cancellationToken);

        //    }
        //}

        var removeTasks = CachedKeys
            .Keys
            .Where(k => k.StartsWith(prefixKey))
            .Select(k => _distributedCache.RemoveAsync(k, cancellationToken));

        await Task.WhenAll(removeTasks);


    }

    public async Task SetAsync<T>(string cacheKey, T value, CancellationToken cancellationToken = default) where T : class
    {
        string cacheValue = JsonSerializer.Serialize(value);

        await _distributedCache.SetStringAsync(cacheKey, cacheValue, cancellationToken);

        CachedKeys.TryAdd(cacheKey, false);
    }
}
