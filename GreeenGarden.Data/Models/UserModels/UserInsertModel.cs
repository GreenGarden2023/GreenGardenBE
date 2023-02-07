using System;
namespace GreeenGarden.Data.Models.UserModels
{
	public class UserInsertModel
	{
        public string UserName { get; set; } = null!;

        public string Password{ get; set; } = null!;

        public string? FullName { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Favorite { get; set; }

        public string? Mail { get; set; }

    }
}

