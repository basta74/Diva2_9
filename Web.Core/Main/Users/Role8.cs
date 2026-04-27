using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Users
{
    public class Role8 : IdentityRole<int>
    {
        //public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// define public by user
        /// </summary>
        public bool PublicAble { get; set; }

        //public virtual IList<UserRoles8> Users { get; set; } = new List<UserRoles8>();
    }
}
