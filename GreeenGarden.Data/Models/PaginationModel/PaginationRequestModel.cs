using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.PaginationModel
{
    public class PaginationRequestModel
    {
        public PaginationRequestModel() {
            pageSize = 5;
            curPage = 1;
        }

        public PaginationRequestModel SearchText(string searchString)
        {
            searchText = searchString;
            return this;
        }

        public PaginationRequestModel PageSize(int size)
        {
            pageSize = size;
            return this;
        }

        public PaginationRequestModel CurPage(int pageNum)
        {
            curPage = pageNum;
            return this;
        }

        public string? searchText { get; set; }

        public int pageSize { get; set; }

        public int curPage { get; set; }

    }
}
