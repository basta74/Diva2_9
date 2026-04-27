using Diva2.Core.Main;
using Diva2.Core.Main.Users;
using Diva2Web.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Zakaznici
{
    public class ZakaznikDetailModel
    {
        public int Id { get; set; }

        public UserModel User { get; set; } = new UserModel();

        public ZakaznikModel Zakaznik { get; set; }

        public Pobocka Pobocka { get; set; }

        public IList<User8Group> SkupinyZakaznika { get; set; }

        public string JsonTransaction { get; set; }

        public string OpenTab { get; set; } = "jsCardTransakcePrijem";

        public Dictionary<string, bool> IniDict { get; set; }
    }
}
