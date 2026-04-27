using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Users;
using Diva2.Services.Managers.Customers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Customers
{
    public interface ISkupinaZakaznikaService
    {
        User8Group GetById(int id);
        IList<User8Group> GetAll();
      
        void Insert(User8Group obj);
        void Update(User8Group obj);
        void Delete(User8Group obk);
    }
}
