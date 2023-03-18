using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.UserTreeModel
{
    public class UserTreeCRUDModel
    {
    }
    public class UserTreeCreateModel
    {
        public string TreeName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public List<string> ImageUrl { get; set; }
    }
    public class UserTreeUpdateModel
    {
        public Guid UserTreeID { get; set; }
        public string TreeName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public List<string> ImageUrl { get; set; }
    }
    public class UserTreeChangeStatusModel
    {
        public Guid UserTreeID { get; set; }
        public string Status { get; set; }
    }
}
