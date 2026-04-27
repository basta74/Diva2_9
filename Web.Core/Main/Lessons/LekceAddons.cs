using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Lessons
{
    // poznamka k lekci
    public class LekceText : BaseEntity
    {
        public string Text { get; set; }


    }

    // video k lekci
    public class LekceVideo : BaseEntity
    {
        public string Url { get; set; }

        public string TextUrl { get; set; }

        public string Text { get; set; }

        public DateTime Dt { get; set; }

    }
}
