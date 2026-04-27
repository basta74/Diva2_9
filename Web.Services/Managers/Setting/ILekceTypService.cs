using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Setting
{
    public interface ILekceTypService
    {
        LekceTyp GetById(int id);
        IList<LekceTyp> GetAll();
      
        void Insert(LekceTyp obj);
        void Update(LekceTyp obj);
        void Delete(LekceTyp obj);
    }
}
