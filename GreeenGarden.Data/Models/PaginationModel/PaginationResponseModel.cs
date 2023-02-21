namespace GreeenGarden.Data.Models.PaginationModel
{
    public class PaginationResponseModel
    {

        public PaginationResponseModel SearchText(string searchString)
        {
            searchText = searchString;
            return this;
        }

        public PaginationResponseModel PageSize(int size)
        {
            pageSize = size;
            return this;
        }

        public PaginationResponseModel CurPage(int pageNum)
        {
            curPage = pageNum;
            return this;
        }

        public PaginationResponseModel RecordCount(int total)
        {
            recordCount = total;
            return this;
        }

        public PaginationResponseModel PageCount(int numOfPage)
        {
            pageCount = numOfPage;
            return this;
        }


        public string? searchText { get; set; }
        public int pageSize { get; set; }
        public int curPage { get; set; }
        public int recordCount { get; set; }
        public int pageCount { get; set; }
    }
}
