using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Trans
{
    public class AddRemoveLessonModel
    {
        public int Id { get; set; }

        public int AktId { get; set; }

        public int UserId { get; set; }

        public int KontCislo { get; set; }

        public string Akce { get; set; }

        public string Text { get; set; }

        public bool Administrace { get; set; } = false;
    }

    public class AddMoneyModel
    {
        public int TypPlatbyId { get; set; }

        public int PlatbaId { get; set; }

        public int UserId { get; set; }

        public int KontCislo1 { get; set; }

        public int KontCislo2 { get; set; }

        public int DoPokladny { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo{ get; set; }


        public string Text { get; set; }
        
    }
}
