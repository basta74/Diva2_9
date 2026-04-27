using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.PayGates
{
    public class PaysItem : BaseEntity
    {
        public int Merchant { get; set; }

        public int Shop { get; set; }

        public int ItemId { get; set; }

        public int TypeId { get; set; }

        public int? PaymentOrderId { get; set; }

        public int Status { get; set; } = 0;

        public string StatusDesc { get; set; }

        public string Hash { get; set; }

        public int Price { get; set; }

        public string Currency { get; set; }

        public string Lang { get; set; }

        public int PokladnaId { get; set; }

        public int UserId { get; set; }

        public int YearMonth { get; set; }

        public string Email { get; set; }

        public DateTime CreatedDt { get; set; }

        public DateTime? UpdatedDt { get; set; }
        public bool AddedCredit { get; set; }

    }
}
