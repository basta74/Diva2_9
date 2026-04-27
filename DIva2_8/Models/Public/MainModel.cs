using System;
using System.Collections.Generic;
using System.Text;
using Diva2.Core.Main;
using Diva2.Core.Main.Lessons;
using Diva2Web.Models.Users;

namespace Diva2Web.Models.Public
{
    public class MainModel : BaseModel
    {
       

        public Dictionary<int, LekceTyden> Rozvrhy { get; set; } = new Dictionary<int, LekceTyden>();

        public IEnumerable<int> Weaks { get; set; }

        

       
    }
}
