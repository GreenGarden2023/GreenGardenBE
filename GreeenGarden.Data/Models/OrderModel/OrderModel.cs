using GreeenGarden.Data.Entities;

namespace GreeenGarden.Data.Models.OrderModel
{
    public class OrderModel
    {
        public List<Item> items { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string address { get; set; }
    }

    public class Item
    {
        public Guid sizeProductItemID { get; set; }
        public int quantity { get; set; }
    }

    public class addendumSizeProductItemRequestModel
    {
        public int quantity { set; get; }
        public Guid sizeProductItemID { get; set; }
    }

    public class addendumToAddByOrderModel
    {
        public Guid orderId { set; get; }
        public DateTime? startDateRent { set; get; }
        public DateTime? endDateRent { set; get; }
        public string? address { set; get; }
        public List<addendumSizeProductItemRequestModel> sizeProductItems { get; set; }

    }

    public class listOrderResponseModel
    {
        public Guid orderId { set; get; }
        public double? totalPrice { set; get; }
        public DateTime? createDate { set; get; }
        public string? status { set; get; }
        public Guid? voucherID { set; get; }
        public bool? isForRent { set; get; }
    }

    // 23.03.05

    public class managerOrderModel
    {
        public Guid orderId { set; get; }
        public double? totalPrice { set; get; }
        public DateTime? createDate { set; get; }
        public string? status { set; get; }
        public user user { set; get; } = new user();
        public bool? isForRent { set; get; }


    }

    public class user { 
        public Guid userID { set; get; }
        public string? userName { set; get; }
        public string? fullName { set; get; }
        public string? address { set; get; }
        public string? phone { set; get; }
        public string? mail { set; get; }
    }

    public class addToOrderModel
    {
        public string? startRentDate { get; set; }
        public string? endRentDate { get; set; }
        public List<itemResponse> rentItems { get; set; }
        public List<itemResponse> saleItems { get; set; }
    }
    public class itemResponse
    {
        public Guid? sizeProductItemID { get; set; }
        public int? quantity { get; set; }
    }


    public class listOrder
    {
        public user user { get; set; }
        public List<orderShowModel> orders { get; set; }

    }

    public class orderShowModel
    {
        public Guid orderID { set; get; }
        public double? totalPrice { get; set; }
        public DateTime? createDate { get; set; }
        public string? status { get; set; }
        public bool isForRent { get; set; }
        public List<addendumShowModel> addendums { get; set; }



    }
    public class addendumShowModel
    {
        public Guid addendumID { set; get; }
        public double? transportFee { set; get; }
        public DateTime? startDateRent { set; get; }
        public DateTime? endDateRent { set; get; }
        public double? deposit { set; get; }
        public double? reducedMoney { set; get; }
        public double? totalPrice { set; get; }
        public string? status { set; get; }
        public double? remainMoney { set; get; }
        public string? address { set; get; }
        public List<addendumProductItemShowModel> addendumProductItems { get; set; }
    }
    public class addendumProductItemShowModel
    {
        public Guid addendumProductItemID { set; get; } 
        public double? sizeProductItemPrice { set; get;}
        public int? Quantity { set; get;}
        public sizeProductItemShowModel? sizeProductItems { set; get; }    
    }
    public class sizeProductItemShowModel
    {
        public Guid sizeProductItemID { set; get; }
        public string? sizeName { set; get; }
        public string? productName { set; get; }
        public List<string>? imgUrl { set; get; } = new List<string>();
    }

    // 23.03.06

    public class orderDetail
    {
        public user user { get; set; }
        public orderShowModel order { get; set; }
    }

}
