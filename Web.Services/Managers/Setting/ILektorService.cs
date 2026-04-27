using Diva2.Core.Main.Lektori;
using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Setting
{
    public interface ILektorService
    {
        Lektor GetById(int id);
        IList<Lektor> GetAll();
      
        void Insert(Lektor obj);
        void Update(Lektor obj);
        void Delete(Lektor obj);


        Mzda GetMzdyuByPokladna(int id);

        void Insert(Mzda obj);
        void Update(Mzda obj);
    }
}
