using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Setting
{
    public interface ILekceMustrService
    {
        LekceMustr GetById(int id);
        IList<LekceMustr> GetAll();
      
        void Insert(LekceMustr obj);
        void Update(LekceMustr obj);

        void Update(IEnumerable<LekceMustr> obj);
        void Delete(LekceMustr obj);
        LekceMustr GetByParams(int pobId, int den, int min, int zdroj);



        #region MystrTyp

        LekceMustrTyp GetTypById(int id);
        IList<LekceMustrTyp> GetTypAll();

        void Insert(LekceMustrTyp obj);
        void Update(LekceMustrTyp obj);
        void Delete(LekceMustrTyp obj);

        #endregion
    }
}
