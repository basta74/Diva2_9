using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Main.Content
{
    public class Page : BaseEntity
    {
        public  PageType Type { get; set; }

        public int Order { get; set; }

        public string UrlName { get; set; }

        public string Controller { get; set; }

        public string Method { get; set; }

        public bool Active { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
    }

    public enum PageType { info, about, video };
}
