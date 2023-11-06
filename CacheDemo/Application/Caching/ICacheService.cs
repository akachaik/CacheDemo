namespace CacheDemo.Application.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default)
        where T : class;

    Task<T> GetAsync<T>(string cacheKey, Func<Task<T>> factory, CancellationToken cancellationToken = default)
        where T : class;
    Task SetAsync<T>(string cacheKey, T value, CancellationToken cancellationToken = default)
        where T : class;

    Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default);

    Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default);
}
