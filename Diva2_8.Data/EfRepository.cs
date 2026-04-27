using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diva2.Core;

namespace Diva2.Data
{
    public partial class EfRepository<T> : IRepository<T> where T : class
    {

        private DbSet<T> _entities;
        private ApplicationDbContext _context;


        public EfRepository(ApplicationDbContext dbContext)
        {
            this._context = dbContext;
        }

        public IQueryable<T> Table => Entities;

        public IQueryable<T> TableUntracked => Entities.AsNoTracking();



        public T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        public void Insert(T entity)
        {
            this.Entities.Add(entity);

            this._context.SaveChanges();
        }

        public void Update(T entity)
        {
            this.Entities.Update(entity);

            this._context.SaveChanges();
        }

        public void Delete(T entity)
        {
            this.Entities.Remove(entity);

            this._context.SaveChanges();
        }

        protected virtual DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<T>();
                }
                return _entities;
            }
        }
    }
}
