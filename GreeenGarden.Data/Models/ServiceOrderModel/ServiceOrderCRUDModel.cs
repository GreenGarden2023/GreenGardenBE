using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.ServiceOrderModel
{
    public class ServiceOrderCRUDModel
    {
    }
    public class ServiceOrderCreateModel
    {
        public Guid ServiceId { get; set; }
        public Guid TechnicianId { get; set; }
        public string? ServiceStartDate { get; set; }
        public string? ServiceEndDate { get; set; }
        public double Incurred { get; set; }
        public double TransportFee { get; set; }
        public string? Description { get; set; }
        public List<ServiceUserTreeCreateModel> UserTrees { get; set; }
    }
    public class ServiceUserTreeCreateModel
    {
        public Guid UserTreeID { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
    public class ServiceUserUpdateModel
    {
        public Guid ServiceOrderId { get; set; }
        public Guid TechnicianId { get; set; }
        public string? ServiceStartDate { get; set; }
        public string? ServiceEndDate { get; set; }
        public double Incurred { get; set; }
        public string? Description { get; set; }
        public List<ServiceUserTreeCreateModel> UserTrees { get; set; }
    }
    public class ServiceOrderChangeStatusModel
    {
        public Guid ServiceOrderId { get; set;}
        public string Status { get; set; }
    }
}
