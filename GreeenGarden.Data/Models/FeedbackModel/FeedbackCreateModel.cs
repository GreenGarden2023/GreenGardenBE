﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.FeedbackModel
{
    public class FeedbackCreateModel
    {
        public Guid ProductItemDetailID  { get; set; }
        public float Rating { get; set; }
        public string Comment { get; set; }
        public List<string> ImagesUrls { get; set; }
        public Guid OrderID { get; set; }

    }
}
