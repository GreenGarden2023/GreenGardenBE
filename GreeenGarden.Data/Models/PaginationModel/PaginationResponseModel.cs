using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.PaginationModel
{
    public class PaginationResponseModel<T>
    {

        public PaginationResponseModel<T> SearchText(string searchString)
        {
            searchText = searchString;
            return this;
        }

        public PaginationResponseModel<T> PageSize(int size)
        {
            pageSize = size;
            return this;
        }

        public PaginationResponseModel<T> CurPage(int pageNum)
        {
            curPage = pageNum;
            return this;
        }

        public PaginationResponseModel<T> Result(List<T> resultList)
        {
            result = resultList;
            return this;
        }
        public PaginationResponseModel<T> RecordCount(int total)
        {
            recordCount = total;
            return this;
        }

        public PaginationResponseModel<T> PageCount(int numOfPage)
        {
            pageCount = numOfPage;
            return this;
        }


        public string? searchText { get; set; }
        public int pageSize { get; set; }
        public int curPage { get; set; }
        public int recordCount { get; set; }
        public int pageCount { get; set; }
        public List<T> result { get; set; }
    }
}
