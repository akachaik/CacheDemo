using System.ComponentModel.DataAnnotations;

namespace CacheDemo.Infrastructure.Caching;

public class CacheSettings
{

    [Range(minimum: 1, int.MaxValue)]
    public int? ExpirationInSeconds { get; set; }
}