using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Model.Json
{
    public class JsonUser : JsonStatus
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Valid { get; set; }

    }
}
