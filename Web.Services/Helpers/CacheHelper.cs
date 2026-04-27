using Diva2.Data.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Diva2.Services
{
    public class CacheHelper : ICacheHelper
    {
        private static IMemoryCache _cache;
        private string subdomain;

        public CacheHelper(IMemoryCache memoryCache, IDomainService domainService)
        {
            _cache = memoryCache;
            subdomain = domainService.Domain?.name;
        }
        public CacheHelper(IMemoryCache memoryCache, string sub)
        {
            _cache = memoryCache;
            subdomain = sub;
        }

        public T GetData<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public T GetDataSub<T>(string key)
        {
            return _cache.Get<T>($"{subdomain}-{key}");
        }

        public void SetData<T>(string key, T data, int minutes = 10)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(minutes));
            _cache.Set(key, data, cacheEntryOptions);
        }

        public void SetDataSub<T>(string key, T data, int minutes = 10)
        {
            SetData<T>($"{subdomain}-{key}", data, minutes);
        }


        public void ClearData(string key)
        {
            _cache.Remove(key);
        }

        public void ClearDataSub(string key)
        {
            _cache.Remove($"{subdomain}-{key}");
        }
    }
}