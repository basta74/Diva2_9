using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Platby
{
    public class PaysResponseModel
    {
        public int PaymentOrderID { get; set; }

        public int PaymentOrderStatusID { get; set; }

        public string MerchantOrderNumber { get; set; }

        public string CurrencyID { get; set; }

        public int Amount { get; set; }

        public int CurrencyBaseUnits { get; set; }

        public string PaymentOrderStatusDescription { get; set; }

        public string Hash { get; set; }
    }



}
