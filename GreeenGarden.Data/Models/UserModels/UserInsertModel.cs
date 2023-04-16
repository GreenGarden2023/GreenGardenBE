using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.UserModels
{
    public class UserInsertModel
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;

        public string? FullName { get; set; }

        public string? Address { get; set; }

        public int? DistrictId { get; set; }
        public string? RoleName { get; set; }

        public string? Phone { get; set; }

        public string? Favorite { get; set; }
        [Required]
        public string? Mail { get; set; }

    }
}

