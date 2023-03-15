using GreeenGarden.Data.Models.CartModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.ServiceModel
{
    public class ServiceCRUDModel
    {
    }
    public class ServiceCreateModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Mail { get; set; }
        //public List<UserTreeCreateModel> userTrees { get; set; }
    }
    public class UserTreeCreateModel
    {
        public Guid UserTreeID { get; set; }
        public int Quantity { get; set; }
    }
    public class ServiceUpdateModel
    {
        public Guid serviceID { get; set; }
        public ServiceCreateModel service { get; set; }
    }
}
