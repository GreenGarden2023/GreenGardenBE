namespace GreeenGarden.Data.Models.UserModels
{
    public class UserLoginResModel
    {
        public Guid ID { get; set; }

        public string UserName { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public byte[] PasswordHash { get; set; } = null!;

        public byte[] PasswordSalt { get; set; } = null!;

        public string RoleName { get; set; } = null!;


    }
}

