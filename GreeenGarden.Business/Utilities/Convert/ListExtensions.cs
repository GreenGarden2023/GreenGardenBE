using EntityFrameworkPaginateCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Utilities.Convert
{
    public static class ListExtensions
    {
        public static Page<T> Paginate<T>(this List<T> list, int pageNumber, int pageSize)
        {
            var totalRecords = list.Count;
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var items = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var result = new Page<T>
            {
                Results = items,
                PageCount = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                RecordCount = totalRecords,
            };
            return result;
        }
    }
}
