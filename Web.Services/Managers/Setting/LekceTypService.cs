using Diva2.Core.Main.Lessons;
using Diva2.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Services.Managers.Setting
{
    public class LekceTypService : ILekceTypService
    {
        private CacheHelper cache;
        private IRepository<LekceTyp> repository;

        public LekceTypService(ApplicationDbContext dbContext, IMemoryCache memoryCache,
                        IRepository<LekceTyp> repTyp)
        {

            this.cache = new CacheHelper(memoryCache, dbContext.SubDomain);


            this.repository = repTyp; 

        }

        public void ClearTypy()
        {
            string cacheItemKey = $"GetTypyLekci";
            cache.ClearDataSub(cacheItemKey);
        }
        public IList<LekceTyp> GetAll()
        {
            string cacheItemKey = $"GetTypyLekci";
            var list = cache.GetDataSub<IList<LekceTyp>>(cacheItemKey);

            if (list == null)
            {
                list = repository.Table.OrderBy(d => d.Nazev).ToList();

                foreach (var typ in list)
                {
                    typ.NazevAdmin = $"{typ.PobockaId}-{typ.Nazev}";
                }

                cache.SetDataSub(cacheItemKey, list);
            }

            return list;
        }

        public LekceTyp GetById(int id)
        {
            return repository.Table.Where(d => d.Id == id).FirstOrDefault();
        }

        public void Delete(LekceTyp obj)
        {
            repository.Delete(obj);
            ClearTypy();
        }

        public void Insert(LekceTyp obj)
        {
            repository.Insert(obj);
            ClearTypy();
        }

        public void Update(LekceTyp obj)
        {
            repository.Update(obj);
            ClearTypy();
        }
    }
}
