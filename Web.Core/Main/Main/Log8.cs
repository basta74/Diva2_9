using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Main
{
    public class Log8 : BaseEntity
    {
        public LogCategory Category { get; set; }

        public string Text { get; set; }

        public DateTime Created { get; set; }
    }

    public enum LogCategory { pay_gate, add_credit, rozvrh_generate }
}
