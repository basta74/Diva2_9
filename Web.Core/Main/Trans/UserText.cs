using Diva2.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Trans
{
    public class UserText : BaseEntity
    {
        public int LekceId { get; set; }

        public int UserId { get; set; }

        public int KontrolniCislo { get; set; }

        public string Text { get; set; }

    }
}
