using Diva2.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Users
{
    public class UserCategory : BaseEntity
    {
        public int PobockaId { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }
    }
}
