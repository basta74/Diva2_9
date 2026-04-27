using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Data
{
    public partial interface IRepository<T> where T : class
    {
        T GetById(object id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> Table { get; }
        IQueryable<T> TableUntracked { get; }
    }
}
