namespace GreeenGarden.Data.Models.ResultModel
{
    public class ResultModel
    {
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public object? Data { get; set; }
        public object? ResponseFailed { get; set; }
        public string? Message { get; set; }
    }


    public class ResponseResult
    {
        public object? Paging { get; set; }
        public object? Result { get; set; }
    }

    public class ProductResponseResult
    {

        public object? Paging { get; set; }
        public object? Category { get; set; }
        public object? Result { get; set; }
    }
    public class ProductItemGetResponseResult
    {

        public object? Paging { get; set; }
        public object? Category { get; set; }
        public object? Product { get; set; }
        public object? ProductItems { get; set; }
    }
    public class ProductItemCreateResponseResult
    {

        public object? Category { get; set; }
        public object? Product { get; set; }
        public object? ProductItems { get; set; }
    }
    public class ProductItemDetailResponseResult
    {

        public object? Category { get; set; }
        public object? Product { get; set; }
        public object? ProductItem { get; set; }
    }
}
