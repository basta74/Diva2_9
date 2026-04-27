using Diva2.Core.Main.Users;
using Diva2.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Services.Managers.Customers
{
    public class SkupinaZakaznikaService : ISkupinaZakaznikaService
    {
        private readonly IRepository<User8Group> repository;
        private readonly CacheHelper cache;

        public SkupinaZakaznikaService(ApplicationDbContext dbContext, IMemoryCache memoryCache, IRepository<User8Group> repository)
        {
            this.repository = repository;
            this.cache = new CacheHelper(memoryCache, dbContext.SubDomain);
        }
        
      
        public IList<User8Group> GetAll()
        {
            string cacheItemKey = $"SkupinyZakazniku";
            IList<User8Group> list = cache.GetDataSub<IList<User8Group>>(cacheItemKey);
            if (list == null)
            {
                list = repository.Table.ToList();
                cache.SetDataSub<IList<User8Group>>(cacheItemKey, list);
            }
            return list;
        }

        public User8Group GetById(int id)
        {
            return repository.Table.Where(d => d.Id == id).FirstOrDefault();
        }

        public void Delete(User8Group obj)
        {
            repository.Delete(obj);
        }


        public void Insert(User8Group obj)
        {
            repository.Insert(obj);
        }

        public void Update(User8Group obj)
        {
            repository.Update(obj);
        }
    }
}
