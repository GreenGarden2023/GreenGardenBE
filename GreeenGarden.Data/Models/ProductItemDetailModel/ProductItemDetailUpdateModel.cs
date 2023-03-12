using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.ProductItemDetailModel
{
    public class ProductItemDetailUpdateModel
    {
    }
    public class ProductItemDetailUpdateStatusModel
    {
        public Guid ProductItemDetailID { get; set; }
        public string Status { get; set; }  
    }
}
