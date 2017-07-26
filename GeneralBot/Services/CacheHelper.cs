using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeneralBot.Services
{
    public class CacheHelper
    {
        private IMemoryCache _cache;

        public CacheHelper(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T2> TryGetValueSet<T1, T2>(T1 key, T2 value, TimeSpan duration)
        {
            T2 cacheEntry;

            // Look for cache key.
            if (!_cache.TryGetValue(key, out cacheEntry))
            {
                cacheEntry = value;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(duration);

                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}