using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Core.Model.Json
{
    public class JsonPlatby : JsonStatus
    {
        public string Name { get; set; }
        public List<PlatbaBaseModel> Items { get; set; } = new List<PlatbaBaseModel>();

    }

    public class PlatbaBaseModel
    {
        public int Id { get; set; }
        public string Popis { get; set; }
        public int Castka { get; set; }
    }
}
