using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.UserTreeModel
{
    public class UserTreeResModel
    {
    }

    // getListByCustomer
    public class UserResModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
    }
    public class UserTreeForListModel
    {
        public Guid Id { get; set; }
        public string TreeName { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public string Status { get; set; }
        public List<string> ImgUrl { get; set; }
    }
    public class ListUserTreeResModel
    {
        public UserResModel User { get; set; } = new UserResModel();
        public List<UserTreeForListModel> UserTrees { get; set; }
    }

    // getDetailByCustomer
    public class DetailUserTreeResModel
    {
        public UserResModel User { get; set; } = new UserResModel   ();
        public UserTreeForListModel UserTrees { get; set; } 
    }
}
