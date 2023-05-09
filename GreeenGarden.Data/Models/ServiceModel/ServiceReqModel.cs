using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.ServiceModel
{
    public class ServiceReqModel
    {
    }
    public class ServiceSearchByCodeModel
    {
        public Guid TechnicianID { get; set; }
        public string ServiceCode { get; set; }

    }
}
