using System;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.UserModels
{
	public class UserUpdateModel
	{
        public string? FullName { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Favorite { get; set; }

        public string? Mail { get; set; }
    }
}

