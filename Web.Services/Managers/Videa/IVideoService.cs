using Diva2.Core.Main.Videa;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Videa
{
    public interface IVideoService
    {
        #region Video
        IEnumerable<Video> GetAll();

        IEnumerable<Video> GetVisible();

        Video GetById(int id);

        void Insert(Video o);

        void Update(Video o);
        #endregion
        
        #region UserVideo

        IEnumerable<UserVideo> GetUserVideos(int id, bool include = false);

        IEnumerable<UserVideo> GetUserVideosAll();

        UserVideo GetUserVideoById(int id);

        void Insert(UserVideo o);

        void Update(UserVideo o);

        #endregion

    }
}
