using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.FeedbackModel
{
    public class FeedbackUpdateModel
    {
    }
    public class FeedbackChangeStatusModel
    {
        public Guid FeedbackID { get; set; }
        public string Status { get; set;}
    }
}
