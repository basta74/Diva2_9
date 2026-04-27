using Diva2.Core.Main.Lessons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Setting
{
    public interface ILekceAddonsService
    {
        LekceVideo GetVideoById(int id);

        void Insert(LekceVideo o);

        void Update(LekceVideo o);

        void Delete(LekceVideo dbObj);

        LekceText GetTextById(int id);

        void Insert(LekceText o);

        void Update(LekceText o);

        void Delete(LekceText o);

    }
}
