using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.EmailCodeVerifyModel
{
    public class EmailCodeVerifyModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? OTPCode { get; set; }
    }
}

