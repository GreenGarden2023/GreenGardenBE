using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.CategoryModel
{
    public class CategoryModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string imgUrl { get; set; }
    }
}
