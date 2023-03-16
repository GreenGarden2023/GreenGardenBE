namespace GreeenGarden.Data.Models.UserModels
{
    public class UserCurrResModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = null!;

        public string? FullName { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Favorite { get; set; }

        public string? Mail { get; set; }

        public string? RoleName { get; set; }

        public string? Token { get; set; }

        public int? CurrentPoint { get; set; }
    }
}

