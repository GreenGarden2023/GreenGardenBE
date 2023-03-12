using GreeenGarden.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.ServiceOrderModel
{
    public class UserModel
    {
    }
    
    public class TechicianModel
    {
        public Guid TechnicianID { get; set; }
        public string Username { get; set; }
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public string Mail { get; set; }
    }
    public class UserResponseModel
    {
        public Guid UserID { get; set; }
        public string Username { get; set; }
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public string? Mail { get; set; }    
    }
}
