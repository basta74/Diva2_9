using Diva2.Core.Main.Lessons;
using Diva2.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Services.Managers.Setting
{
    public class LekceMustrService : ILekceMustrService
    {
        private CacheHelper cache;
        private IRepository<LekceMustr> repository;
        private IRepository<LekceMustrTyp> repTyp;

        public LekceMustrService(ApplicationDbContext dbContext, IMemoryCache memoryCache, IRepository<LekceMustr> repMustr, IRepository<LekceMustrTyp> repMustrT)
        {
            this.cache = new CacheHelper(memoryCache, dbContext.SubDomain);
            this.repository = repMustr;
            this.repTyp = repMustrT;
        }

        public IList<LekceMustr> GetAll()
        {
            return repository.Table.ToList();
        }

        public LekceMustr GetById(int id)
        {
            return repository.Table.Where(d => d.Id == id).FirstOrDefault();
        }

        public void Delete(LekceMustr obj)
        {
            repository.Delete(obj);
        }

        public void Insert(LekceMustr obj)
        {
            repository.Insert(obj);
        }

        public void Update(LekceMustr obj)
        {
            repository.Update(obj);
        }

        public void Update(IEnumerable<LekceMustr> obj)
        {
            foreach (var o in obj)
            {
                repository.Update(o);
            }
        }

        public LekceMustr GetByParams(int pobId, int den, int min, int zdroj)
        {
            return repository.Table.Where(d => d.PobockaId == pobId && d.Den == den && d.MinutaKey == min && d.Zdroj == zdroj).FirstOrDefault();
        }

        #region MustrTyp

        public void ClearTypy()
        {
            string cacheItemKey = $"GetTypyLekciMustr";
            cache.ClearDataSub(cacheItemKey);
        }

        public IList<LekceMustrTyp> GetTypAll()
        {
            string cacheItemKey = $"GetTypyLekciMustr";
            var list = cache.GetDataSub<IList<LekceMustrTyp>>(cacheItemKey);

            if (list == null)
            {
                list = repTyp.Table.OrderBy(d => d.Nazev).ToList();
                cache.SetDataSub(cacheItemKey, list);
            }

            return list;
        }

        public LekceMustrTyp GetTypById(int id)
        {
            return repTyp.Table.Where(d => d.Id == id).FirstOrDefault();
        }

        public void Delete(LekceMustrTyp obj)
        {
            repTyp.Delete(obj);
            ClearTypy();
        }

        public void Insert(LekceMustrTyp obj)
        {
            repTyp.Insert(obj);
            ClearTypy();
        }

        public void Update(LekceMustrTyp obj)
        {
            repTyp.Update(obj);
            ClearTypy();
        }



        #endregion
    }
}
