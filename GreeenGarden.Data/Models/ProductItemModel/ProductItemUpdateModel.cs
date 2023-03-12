using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.ProductItemModel
{
    public class ProductItemUpdateModel
    {
    }
    public class ProductItemDetailUpdateStatusModel
    {
        public Guid ProductItemDetailId { get; set; }
        public string Status { get; set; }
    }
}
