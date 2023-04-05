namespace GreeenGarden.Data.Models.UserTreeModel
{
    public class UserTreeUpdateModel
    {
        public Guid Id { get; set; }

        public string? TreeName { get; set; }

        public string? Description { get; set; }

        public int? Quantity { get; set; }

        public string? Status { get; set; }

        public List<string>? ImgUrls { get; set; }
    }
}

