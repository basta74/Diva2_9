using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Model.Json
{
    public class JsonAddMoney : JsonStatus
    {
        public int TypPlatbyId { get; set; }

        public int PlatbaId { get; set; }

        public int UserId { get; set; }

        public int ProvedlId { get; set; }

        public int DoPokladny { get; set; }
        public int Kredity { get; set; }
        public string KredityCasove { get; set; }

    }
}
