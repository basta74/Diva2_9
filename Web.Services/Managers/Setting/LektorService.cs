using Diva2.Core.Main.Lektori;
using Diva2.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Services.Managers.Setting
{
    public class LektorService : ILektorService
    {
        private readonly IRepository<Lektor> repository;
        private readonly IRepository<Mzda> repMzda;
        private readonly ICacheHelper cache;

        public LektorService(ApplicationDbContext dbContext, IRepository<Lektor> repository, IRepository<Mzda> reposMzda, ICacheHelper memoryCache)
        {

            this.repository = repository;
            this.repMzda = reposMzda;
            this.cache = memoryCache;
        }

        public void ClearLektory()
        {
            string cacheItemKey = $"GetLektory";
            cache.ClearDataSub(cacheItemKey);
        }

        public IList<Lektor> GetAll()
        {
            string cacheItemKey = $"GetLektory";

            var list = cache.GetDataSub<IList<Lektor>>(cacheItemKey);

            if (true)
            {
                list = repository.Table.ToList();
                cache.SetDataSub(cacheItemKey, list);
            }

            return list;

        }

        public Lektor GetById(int id)
        {
            return repository.Table.Where(d => d.Id == id).FirstOrDefault();
        }

        public void Update(Lektor o)
        {
            repository.Update(o);
            ClearLektory();
        }
        public void Insert(Lektor o)
        {
            repository.Insert(o);
            ClearLektory();
        }
        public void Delete(Lektor o)
        {
            repository.Delete(o);
            ClearLektory();
        }


        public Mzda GetMzdyuByPokladna(int id)
        {
            var mzda = repMzda.Table.Where(d => d.PokladnaId == id).FirstOrDefault();
            if (mzda == null) {

                mzda = new Mzda() { PokladnaId = id };
                Insert(mzda);
            }
            return mzda;
        }

        public void Insert(Mzda obj)
        {
            repMzda.Insert(obj);
        }

        public void Update(Mzda obj)
        {
            repMzda.Update(obj);
        }
    }
}
