using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.UserModels
{
    public class PasswordResetModel
    {
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string OTPCode { get; set; }
    }
}

