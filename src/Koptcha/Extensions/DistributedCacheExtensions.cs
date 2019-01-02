using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace Koptcha.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static void Set<T>(this IDistributedCache distributedCache, string key, T value,
            DistributedCacheEntryOptions options)
        {
            distributedCache.Set(key, value.ToByteArray(), options);
        }

        public static T Get<T>(this IDistributedCache distributedCache, string key)
        {
            var result = distributedCache.Get(key);
            return result.FromByteArray<T>();
        }

        public static async Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            await distributedCache.SetAsync(key, value.ToByteArray(), options, token);
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken token = default(CancellationToken)) where T : class
        {
            var result = await distributedCache.GetAsync(key, token);
            return result.FromByteArray<T>();
        }
    }
}