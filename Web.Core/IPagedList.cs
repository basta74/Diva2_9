using System.Collections.Generic;

namespace Diva2.Core
{
    public interface IPagedList<T> where T : class
    {
        List<T> Data { get; set; }
        long PageNumber { get; set; }
        int PageSize { get; set; }
        long TotalItems { get; set; }
    }
}