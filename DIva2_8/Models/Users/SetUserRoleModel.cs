using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Users
{
    public class SetUserRoleModel
    {

        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int Value { get; set; }
    }
}
