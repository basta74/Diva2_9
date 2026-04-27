using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Users
{
    public class UserRoles8 : IdentityUserRole<int>
    {
        //public int UserId { get; set; }
        //public int RoleId { get; set; }

        
        public override int UserId { get; set; }

        public virtual User8 User { get; set; }
        public override int RoleId { get; set; }

        public virtual Role8 Role { get; set; }
        /**/
    }
}
