﻿namespace GreeenGarden.Data.Models.ProductModel
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public Guid? CategoryId { get; set; }
        public string? ImgUrl { get; set; }
    }


}
