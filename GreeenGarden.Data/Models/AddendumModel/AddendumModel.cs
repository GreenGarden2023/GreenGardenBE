using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.AddendumModel
{
    public class AddendumModel
    {
    }
    public class AdddendumResponseModel
    {
        public Guid id { get; set; }
        public double? transportFee { get; set; }
        public DateTime? startDateRent { get; set; }
        public DateTime? endDateRent { get; set; }
        public double? deposit { get; set; }
        public double? reducedMoney { get; set; }
        public double? totalPrice { get; set; }
        public string? status { get; set; }
        public Guid orderID { get; set; }
        public double? remainMoney { get; set; }
        public List<addendumProductItemResponseModel>? sizeProductItems { get; set; }
    }
    public class addendumProductItemResponseModel
    {
        public double? sizeProductItemPrice { get; set; }
        public int? quantity { set; get; }
        public Guid? sizeProductItemID { get; set; }
    }

    public class listAddendumResponse
    {
        public Guid id { set; get; }
        public Guid orderID { set; get; }
        public DateTime? startDateRent { set; get; }
        public DateTime? endDateRent { set; get; }
        public string? status { set; get; }
        public double? deposit { set; get; }
        public double? totalPrice { set; get; }
        public double? remainMoney { set; get; }
        public List<addendumProductItemResponseModel>? productItems { get; set; }
    }
}
