using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Users
{
    public class AccountEditModel
    {
        public List<Role8> Roles = new List<Role8>();

        public User8 User = new User8();

        public List<UserRoles8> UserRoles = new List<UserRoles8>();


    }
}
