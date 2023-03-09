using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.SizeModel
{
    public class SizeCreateModel
    {
        [Required]
        public string SizeName { get; set; }
        public bool? SizeType { get; set; }
    }
    public class SizeUpdateModel
    {
        public Guid SizeID { get; set; }
        public string? SizeName { get; set; }
        public bool? SizeType { get; set; }
    }

}

