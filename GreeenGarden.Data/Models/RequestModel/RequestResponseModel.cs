using GreeenGarden.Data.Models.ServiceOrderModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.RequestModel
{
    
    public class RequestResponseModel
    {
        public UserResponseModel user { get; set; } = new UserResponseModel();
        public List<RequestResponse> request { get; set; }

    }

    public class RequestResponse
    {
        public Guid RequestID { get; set; }
        public Guid UserID { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Status { get; set; }
        public List<RequestDetailResponse> RequestDetail { get; set; }
    }

    public class RequestDetailResponse
    {
        public Guid RequestDetailID { get; set;}
        public string TreeName { get; set;}
        public int? Quantity { get; set;}
        public string? Description { get; set;}
        public List<string> ImageUrl { get; set; } = new List<string> ();
    }
}
