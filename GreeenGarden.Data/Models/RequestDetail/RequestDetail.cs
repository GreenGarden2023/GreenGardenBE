using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.RequestDetail
{
    public class RequestDetail
    {
        public string TreeName { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public Guid RequestID   { get; set; }
    }
}
