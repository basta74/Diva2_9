using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Trans
{
    /// <summary>
    /// Log o prodlouzeni
    /// </summary>
    public class UserZbytekKreditCasLog : BaseEntity
    {
        /// <summary>
        /// Provedl
        /// </summary>
        public int UserId { get; set; }

        public int PlatbaId { get; set; }

        public int UnixFrom { get; set; }

        public int UnixTo { get; set; }

        public int Days { get; set; }

        public DateTime Ts { get; set; }
        
    }
}
