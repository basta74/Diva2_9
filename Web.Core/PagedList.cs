using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diva2.Core
{
    public class PagedList<T> : IPagedList<T> where T : class
    {
        public PagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            TotalItems = source.Count();
            int offset = (pageSize * pageNumber) - pageSize;
            if (offset < 0) {
                offset = 0;
            }
            PageNumber = pageNumber;
            PageSize = pageSize;
            Data = source.Skip(offset).Take(pageSize).ToList();
        }
        public List<T> Data { get; set; }
        public long TotalItems { get; set; }
        public long PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
