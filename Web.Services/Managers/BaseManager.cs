using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Data;
using Microsoft.AspNetCore.Http;

namespace Diva2.Services.Managers
{
    public class BaseManager
    {
        protected ApplicationDbContext dbContext;
        protected static CacheHelper cache;
        protected readonly IHttpContextAccessor httpContextAccessor;

        public BaseManager(ApplicationDbContext dbContext, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            cache = new CacheHelper(memoryCache, dbContext.SubDomain);
            this.httpContextAccessor = httpContextAccessor;

            
        }
    }
}
