using Expenses.Application.services;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Expenses.Infrastructure.services
{
    public class CacheService<T> : ICacheService<T>
    {
        private readonly HybridCache _cache;
        private readonly string _cacheKey;
        private readonly ILogger<CacheService<T>> _logger;
        public CacheService(HybridCache cache, ILogger<CacheService<T>> logger)
        {
            _cache = cache;
            _cacheKey = GetCacheKey<T>();
            _logger = logger;
        }
        public async Task<T> GetOrCreate(Func<CancellationToken, Task<T>> cacheDelegate)
        {
            return await _cache.GetOrCreateAsync(_cacheKey,
                async (ct) => {
                    _logger.LogInformation($"Cache MISS: {_cacheKey}");
                    return await cacheDelegate(ct); 
                }
                , default);
        }
        public async Task<T> Invalidate()
        {
            await _cache.RemoveAsync(_cacheKey);
            return default!;
        }

        private string GetCacheKey<T>()
        {
            var type = typeof(T);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var element = type.GetGenericArguments().ToList().FirstOrDefault();
                if (element != null)
                {
                    return element.Name ?? throw new InvalidOperationException("Cannot determine cache key for type " + element.Name);
                }
            }
            return type.Name ?? throw new InvalidOperationException("Cannot determine cache key for type " + type.Name);
        }
    }
}