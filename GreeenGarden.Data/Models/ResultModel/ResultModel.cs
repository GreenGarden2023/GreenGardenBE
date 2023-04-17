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
    public class RentOrderGroupResModel
    {
        public object? Paging { get; set; }
        public object? RentOrderGroups { get; set; }
    }
    public class SaleOrderGetResModel
    {
        public object? Paging { get; set; }
        public object? SaleOrderList { get; set; }
    }

    public class GetARentOrderRes
    {
        public object? RentOrder { get; set; }
        public object? ProductItemDetailList { get; set; }
    }

    public class ServiceOrderListRes
    {
        public object? Paging { get; set; }
        public object? ServiceOrderList { get; set; }
    }
    public class RequestListRes
    {
        public object? Paging { get; set; }
        public object? RequestList { get; set; }
    }
    public class FeedbackRes
    {
        public object? Paging { get; set; }
        public object? FeedbackList { get; set; }
    }
    public class RentOrderByRangeDateResModel
    {
        public object? Paging { get; set; }
        public object? RentOrderGroups { get; set; }
    }
}
