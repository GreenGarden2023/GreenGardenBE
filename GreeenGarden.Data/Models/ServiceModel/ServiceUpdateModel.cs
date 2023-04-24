using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.ServiceModel
{
    public class ServiceUpdateModel
    {
    }
    public class CancelRequestModel
    {
        public Guid serviceID { get; set; }
        public string? reason { get; set; }
    }
}
