using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.ServiceModel
{
    public class UserResModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
    }

    public class UserTreeResModel
    {
        public Guid Id { get; set; }
        public string TreeName { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public string Status { get; set; }
        public List<string> ImageUrl { get; set; } 
    }
    public class ServiceUserTreeRespModel {
        public Guid Id { get; set; }
        public UserTreeResModel UserTree { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
    }

    // getList
    public class ListServiceByCustomerResModel
    {
        public UserResModel User { get; set; } = new UserResModel();
        public List<ServiceForListResModel> Services { get; set; }

    }
    public class ServiceForListResModel
    {
        public Guid Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public List<ServiceUserTreeRespModel> UserTrees { get; set; }
    }
    public class ServiceByManagerResModel
    {
        public Guid Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public UserResModel User { get; set; }
        public List<ServiceUserTreeRespModel> UserTrees { get; set; }
    }

    //getDetail
    public class DetailServiceByCustomerResModel
    {
        public UserResModel User { get; set; } = new UserResModel();
        public ServiceForListResModel Services { get; set; }

    }

}
