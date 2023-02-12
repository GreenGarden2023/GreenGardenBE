using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.ProductModel
{
    public class ProductModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public int? quantity { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public Guid categoryId { get; set; }
        public string imgUrl { get; set; }
    }
    
    public class ProductCreateRequestModel
    {
        public string name { get; set; }
        public string? description { get; set; }
        public Guid categoryId { get; set; }
    }
}
