using Diva2.Core.Main.Trans;
using Diva2.Core.Model.Json;
using Diva2Web.Models.Users;
using Diva2Web.Models.Zakaznici;
using System.Collections.Generic;

namespace Diva2Web.Models.Responses
{
    public class JsonZakaznici : JsonStatus
    {
        public List<ZakaznikModel> Items { get; set; } = new List<ZakaznikModel>();

        public int ItemsCount { get { return Items.Count; } }
    }

    public class JsonObjednavky : JsonStatus
    {
        public List<JsonObjednavka> Items { get; set; } = new List<JsonObjednavka>();

        public int ItemsCount { get { return Items.Count; } }
    }

    public class JsonObjednavka
    {
        private LekceUser lu;

        public int id { get; set; }

        public string d { get; set; }

        public string  h { get; set; }

        public JsonObjednavka(LekceUser lu)
        {
            d = lu.Lekce.Datum.ToString("dd.MM.yyyy");
            h = lu.Lekce.Cas.ToString();
            id = lu.Lekce.Id;
        }
    }
}
