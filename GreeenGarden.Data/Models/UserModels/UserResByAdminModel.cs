using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.UserModels
{
    public class UserResByAdminModel
    {
        public Guid ID { get; set; }
        public string UserName { get; set; } 
        public string FullName { get; set; } 
        public string Address { get; set; } 
        public string DistrictName { get; set; } 
        public int? DistrictID{ get; set; } 
        public string Phone { get; set; } 
        public string Favorite { get; set; } 
        public string Mail { get; set; } 
        public string RoleName { get; set; } 
        public string Status { get; set; } 

    }
}
