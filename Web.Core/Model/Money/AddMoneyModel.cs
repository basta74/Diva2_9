using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Model.Money
{
    public class AddMoneyTrans
    {
        public int TypPlatbyId { get; set; }

        public int PlatbaId { get; set; }

        public int UserId { get; set; }

        public int ProvedlId { get; set; }

        public int DoPokladny { get; set; }

        public int PokladnaId { get; set; }
        
        public bool ZBanky { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Plus - true=přidávám , false=odebírám
        /// </summary>
        public bool Plus { get; set; } = true;
    }
}
