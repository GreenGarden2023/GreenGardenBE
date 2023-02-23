using System;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.SizeModel
{
	public class SizeCreateModel
	{ 
		[Required]
		public string SizeName { get; set; }
	}
}

