using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.UserModels
{
    public class PasswordResetModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? NewPassword { get; set; }
        [Required]
        public string? OTPCode { get; set; }
    }
    public class UserSupportModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
    }
}

