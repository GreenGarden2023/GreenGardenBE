using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.RequestModel
{
    public class RequestCreateModel
    {
        public string Address { get; set; }
        public string Phone { get; set; }
        public List<RequestDetailCreateModel> RequestDetail { get; set; }
    }

    public class RequestDetailCreateModel
    {
        public string TreeName { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public List<string> ImageUrl { get; set; }
    }

    public class RequestUpdateStatusModel
    {
        public  Guid RequestID { get; set; }
        public string Status { get; set; }
    }
}
