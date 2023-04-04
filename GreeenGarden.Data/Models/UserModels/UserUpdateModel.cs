namespace GreeenGarden.Data.Models.UserModels
{
    public class UserUpdateModel
    {
        public string? FullName { get; set; }

        public string? Address { get; set; }

        public int? DistrictID { get; set; }

        public string? Phone { get; set; }

        public string? Favorite { get; set; }

        public string? Mail { get; set; }
    }
    public class OTPVerifyModel
    {
        public string Email { get; set; }
        public string OTPCode { get; set; }
    }
    public class UserUpdateStatusModel
    {
        public Guid UserID { get; set; }
        public string Status { get; set; }
    }
}

