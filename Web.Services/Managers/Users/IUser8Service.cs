using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Users;
using Diva2.Core.Main.Zakaznik;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Services.Managers.Users
{
    public interface IUser8Service
    {
        User8 GetById(int id);
        List<string> GetRules(int id);
        User8 GetByNamePassword(string nick, string heslo);

        IList<User8> GetAll();

        IList<User8> GetByIds(IEnumerable<int> id);

        IList<User8> GetByRole(int id);

        IList<User8> GetForRegister(string email);

        IList<User8> GetByEmail(string email);

        IList<User8> GetDeletedUsers();

        void Update(User8 user);
        void Insert(User8 us);
        
        void Delete(User8 user);

    }
}
