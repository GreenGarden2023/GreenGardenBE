﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.CategoryModel
{
    public class CategoryModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? ImgUrl { get; set; }
        public string? Description { get; set; }
    }
}
