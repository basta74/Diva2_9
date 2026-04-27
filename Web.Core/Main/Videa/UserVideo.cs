using Diva2.Core.Main.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Videa
{
    public class UserVideo : BaseEntity
    {
        public User8 User { get; set; }
        public int UserId { get; set; }

        public int VideoId { get; set; }

        public string Nazev { get; set; }

        public string Url { get; set; }

        public string Image { get; set; }

        public Video Video { get; set; }

        public int Kredity { get; set; }

        public bool Zaplaceno { get; set; }

        public DateTime ZaplacenoDt { get; set; }

        public bool Aktivovano { get; set; }

        public DateTime AktivovanoDt { get; set; }

        public bool Zobrazeno { get; set; }

        public DateTime ZobrazenoDt { get; set; }

    }
}
