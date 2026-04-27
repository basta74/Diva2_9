using Diva2.Core.Main.Main;
using Diva2.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diva2.Services.Managers.Mains
{
    public class Logs8Service : ILogs8Service
    {
        private readonly IRepository<Log8> repository;

        public Logs8Service(IRepository<Log8> repository)
        {
            this.repository = repository;
        }

        public void Insert(Log8 o)
        {
            repository.Insert(o);
        }
    }
}
