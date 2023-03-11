using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.RequestModel
{
    public class RequestCreateModel
    {
        public Guid UserID { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }  
        public List<RequestDetailCreateModel> requestDetails { get; set; }
        
    }
    public class RequestDetailCreateModel
    {
        public string TreeName { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
