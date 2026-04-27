using Diva2.Core.Main.Content;
using Diva2.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Services.Managers.Content
{
    public class PageService : IPageService
    {
        private readonly CacheHelper cache;
        private readonly IRepository<Page> repository;

        public PageService(ApplicationDbContext dbContext, IRepository<Page> repository, IMemoryCache memoryCache)
        {
            this.cache = new CacheHelper(memoryCache, dbContext.SubDomain);
            this.repository = repository;
        }

        private void ClearPages() {

            string cacheItemKey = $"Pages";
            cache.ClearDataSub(cacheItemKey);
        }

        public IEnumerable<Page> GetAll()
        {
            string cacheItemKey = $"Pages";
            IList<Page> list = cache.GetDataSub<IList<Page>>(cacheItemKey);
            if (list == null)
            {
                list = repository.TableUntracked.ToList();
                cache.SetDataSub<IList<Page>>(cacheItemKey, list);
            }
            return list;

        }

        private void ClearPagesVisible()
        {

            string cacheItemKey = $"PagesVisible";
            cache.ClearDataSub(cacheItemKey);
        }

        public IEnumerable<Page> GetVisibleForMenu()
        {
            string cacheItemKey = $"PagesVisible";
            IList<Page> list = cache.GetDataSub<IList<Page>>(cacheItemKey);
            if (list == null)
            {
                list = repository.TableUntracked.Where(d => d.Active == true).ToList();
                foreach (var i in list) {
                    i.Title = null;
                    i.Content = null;
                }
                cache.SetDataSub<IList<Page>>(cacheItemKey, list);
            }
            return list;

        }

        private void ClearPageByType(PageType t)
        {

            string cacheItemKey = $"Page-{t}";
            cache.ClearDataSub(cacheItemKey);
        }

        public Page GetByType(PageType t)
        {
            string cacheItemKey = $"Page-{t}";
            Page p = cache.GetDataSub<Page>(cacheItemKey);
            if (p == null)
            {
                p = repository.TableUntracked.Where(d => d.Type == t).FirstOrDefault();
               
                cache.SetDataSub<Page>(cacheItemKey, p);
            }
            return p;

          
        }

        public Page GetById(int id)
        {
            return repository.TableUntracked.Where(d => d.Id == id).FirstOrDefault();
        }


        public void Update(Page p)
        {
            repository.Update(p);
            ClearPages();
            ClearPagesVisible();
            ClearPageByType(p.Type);
        }
    }
}
