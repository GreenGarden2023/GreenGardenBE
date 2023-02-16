using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.SubProductModel
{
    public class SubProductModel
    {
    }
    public class SizeItemRequestModel
    {
        public Guid productId { get; set; }
        public Guid sizeId { get; set; }
        public string name { get; set; }
        public string price { get; set; }

    }
}
