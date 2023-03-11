using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.ServiceOrderModel
{
    public class ServiceOrderModel
    {
    }
    public class ServiceOrderCreateModel
    {
        public Guid RequestID { get; set; }
        public Guid TechnicianID { get; set; }
        public string ServiceStartDate { get; set; }
        public string ServiceEndDate { get; set; }
        public List<RequestDetailModel> RequestDetailModel { get; set; }
    }
    public class ServiceOrderResponseModel
    {
    }
}
