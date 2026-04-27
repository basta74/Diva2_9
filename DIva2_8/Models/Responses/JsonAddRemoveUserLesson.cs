using Diva2.Core.Main.Lessons;
using Diva2.Core.Main.Platby;
using Diva2.Core.Main.Trans;
using Diva2.Core.Model.Json;
using Diva2Web.Models.Lekces;
using Diva2Web.Models.Platby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diva2Web.Models.Responses
{
    public class JsonAddRemoveUserLesson : JsonStatus
    {
        public int LekceId { get; set; }

        public bool Nahradnik { get; set; }

        public bool NahradnikJa { get; set; }

        public int PosliSms { get; set; }

        public bool ZustaneVLekci { get; internal set; }

        public Lekce Lekce { get; set; }

    }

    public class JsonUserLessonOrDay : JsonStatus
    {
        public IList<LekceUserModel> Zakaznici { get; set; }
    }

    public class JsonLessonHistory : JsonStatus
    {
        public IEnumerable<UserLekceChange> LekceUserChange { get; set; }
    }

    public class JsonAddMoney : JsonStatus
    {
        public int Kredity { get; set; }

        public string KredityCasove { get; set; }

    }
}
