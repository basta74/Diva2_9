using Diva2.Core.Main.Videa;
using Diva2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Services.Managers.Videa
{
    public class VideoService : IVideoService
    {
        private CacheHelper cache;
        private IRepository<Video> repository;
        private IRepository<UserVideo> repUV;

        public VideoService(ApplicationDbContext dbContext, IMemoryCache memoryCache,
                        IRepository<Video> rep, IRepository<UserVideo> reUV)
        {

            this.cache = new CacheHelper(memoryCache, dbContext.SubDomain);
            this.repository = rep;
            this.repUV = reUV;
        }

        #region Video
        public IEnumerable<Video> GetAll()
        {
            return repository.TableUntracked;
        }

        public IEnumerable<Video> GetVisible()
        {
            return repository.TableUntracked.Where(d => d.Visible == true);
        }

        public Video GetById(int id)
        {
            return repository.GetById(id);
        }


        public void Insert(Video o)
        {
            repository.Insert(o);
        }

        public void Update(Video o)
        {
            repository.Update(o);
        }


        #endregion

        #region UserVideos
        public IEnumerable<UserVideo> GetUserVideos(int id, bool include = false)
        {
            if (include)
            {
                return repUV.TableUntracked.Where(d => d.UserId == id).Include(d => d.Video);
            }
            else
            {
                return repUV.TableUntracked.Where(d => d.UserId == id);
            }
        }

        public IEnumerable<UserVideo> GetUserVideosAll()
        {
            return repUV.TableUntracked.Include(d => d.Video).Include(d => d.User).OrderByDescending(d => d.Id);
        }

        public UserVideo GetUserVideoById(int id)
        {
            return repUV.TableUntracked.Include(d => d.Video).FirstOrDefault(d => d.Id == id);
        }

        public void Insert(UserVideo o)
        {
            repUV.Insert(o);
        }

        public void Update(UserVideo o)
        {
            repUV.Update(o);
        }
        #endregion

    }
}
