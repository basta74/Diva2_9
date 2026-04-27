using Diva2.Core.Main;
using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core
{
    public interface IWorkContext
    {
        User8 CurrrentUser { get; set; }
        Pobocka CurentPobocka { get; set; }
        string CurrentDomain { get; set; }

    }
}
