using Diva2.Core.Main.Lessons;
using Diva2.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Setting
{
    public class LekceAddonsService : ILekceAddonsService
    {
        private readonly IRepository<LekceVideo> repVideo;
        private readonly IRepository<LekceText> repText;

        protected static CacheHelper cache;
        public LekceAddonsService(ApplicationDbContext dbContext, IRepository<LekceVideo> repVi, IRepository<LekceText> repTe, IMemoryCache memoryCache)
        {
            cache = new CacheHelper(memoryCache, dbContext.SubDomain);
            this.repVideo = repVi;
            this.repText = repTe;
        }

        public LekceVideo GetVideoById(int id)
        {
            return repVideo.GetById(id);
        }

        public void Insert(LekceVideo o)
        {
            repVideo.Insert(o);
        }
    
        public void Update(LekceVideo o)
        {
            repVideo.Update(o);
        }

        public void Delete(LekceVideo o)
        {
            repVideo.Delete(o);
        }


        public LekceText GetTextById(int id)
        {
            return repText.GetById(id);
        }

        public void Insert(LekceText o)
        {
            repText.Insert(o);
        }

        public void Update(LekceText o)
        {
            repText.Update(o);
        }

        public void Delete(LekceText o)
        {
            repText.Delete(o);
        }

    }
}
