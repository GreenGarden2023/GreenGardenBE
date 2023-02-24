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
        public Guid Id { get; set; }
        public double? TransportFee { get; set; }
        public DateTime? StartDateRent { get; set; }
        public DateTime? EndDateRent { get; set; }
        public double? Deposit { get; set; }
        public double? ReducedMoney { get; set; }
        public double? TotalPrice { get; set; }
        public string? Status { get; set; }
        public Guid OrderID { get; set; }
        public List<addendumProductItemResponseModel>? ProductItems { get; set; } 
    }
    public class addendumProductItemResponseModel
    {
        public double? ProductItemPrice { get; set; }
        public int? Quantity { set; get; }
        public Guid? ProductItemID { get; set; }
    }

}
