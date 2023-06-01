using System;
namespace GreeenGarden.Data.Models.TakecareComboModel
{
	public class TakecareComboModel
	{
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public double Price { get; set; }

        public string? Description { get; set; }

        public string? Guarantee { get; set; }
        public string? CareGuide { get; set; }

        public bool? Status { get; set; }
    }
    public class TakecareComboInsertModel
    {
        public string Name { get; set; } = null!;

        public double Price { get; set; }

        public string? Description { get; set; }

        public string? Guarantee { get; set; }
        public string? CareGuide { get; set; }
    }
    public class TakecareComboUpdateModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double? Price { get; set; }

        public string? Description { get; set; }

        public string? Guarantee { get; set; }

        public bool? Status { get; set; }
    }
}

