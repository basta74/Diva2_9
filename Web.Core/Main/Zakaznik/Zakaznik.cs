using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Zakaznik
{
    public class Zakaznik
    {
        
        
        public User8 User { get; set; }

        public int PocetLekci { get; set; }

        public Zakaznik(User8 user)
        {
            this.User = user;
        }

        public Zakaznik()
        {
        }
    }
}
