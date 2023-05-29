using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ProductItemDetailModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.RevenueModel
{
    public class RevenueResModel
    {
    }
    public class RevenueResByDateModel
    {
        public double? totalRevenue { get; set; }
        public double? rentRevenue { get; set; }
        public double? saleRevenue { get; set; }
        public double? serviceRevenue { get; set; }
        public double? serviceComboRevenue { get; set; }
    }

    public class ProductItemDetailRevenueResModel
    {
        public Guid? productItemDetailId { get; set; }
        public int quantity { get; set; }
        public double? revenueProductItemDetail { get; set; }
    }
    public class TblProductItemDetailRevenueResModel
    {
        public ProductItemDetailResModel? productItemDetail { get; set; }
        public int quantity { get; set; }
        public double? revenueProductItemDetail { get; set; }
    }

    public class rentRevenueResModel
    {
        public int orderNumer { get; set; }
        public double? rentRevenue { get; set; }
        public List<TblProductItemDetailRevenueResModel> itemDetailRevenue { get; set; }
    }
    public class revenueByMonthResModel
    {
        public int month { get; set; }
        public RevenueResByDateModel revenues { get; set; }
    }

}
