using GreeenGarden.Data.Entities;
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
        public UserResponseModel User { get; set; }
        public List<ServiceOrderShowModel> ServiceOrders { get; set; }   


    }
    public class ServiceOrderShowModel
    {
        public Guid ServiceOrderID { get; set;}
        public DateTime? CreateDate { get; set;}
        public DateTime? ServiceStartDate { get; set;}
        public DateTime? ServiceEndDate { get; set;}
        public double? Deposit { get; set;}
        public double? TotalPrice { get; set;}
        public string? Status { get; set;}
        public int? RewardPointGain { get; set;}
        public int? RewardPointUsed { get; set;}
        public TechicianModel Technician { get; set;}
        public List<RequestDetailModel> requestDetails { get; set; }
    }


}
