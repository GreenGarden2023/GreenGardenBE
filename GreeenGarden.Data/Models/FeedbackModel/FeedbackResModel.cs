using GreeenGarden.Data.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.FeedbackModel
{ 
    public class ListFeedbackResModel
    {
        public Guid ID { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; }
        public UserCurrResModel User { get; set; }


    }
}
