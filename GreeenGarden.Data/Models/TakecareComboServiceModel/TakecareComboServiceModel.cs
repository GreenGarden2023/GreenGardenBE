﻿using System;
using GreeenGarden.Data.Entities;

namespace GreeenGarden.Data.Models.TakecareComboServiceModel
{
	public class TakecareComboServiceModel
	{
        public Guid Id { get; set; }

        public string Code { get; set; } = null!;

        public TakecareComboServiceDetail? TakecareComboDetail { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public Guid UserId { get; set; }

        public Guid? TechnicianId { get; set; }

        public string? TechnicianName { get; set; }

        public int TreeQuantity { get; set; }

        public string Status { get; set; } = null!;
    }
    public class TakecareComboServiceInsertModel
    {
        public Guid TakecareComboId { get; set; }

        public string StartDate { get; set; }

        public int NumOfMonth { get; set; }

        public bool IsAtShop { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public int TreeQuantity { get; set; }
    }
    public class TakecareComboServiceChangeStatusModel
    {
        public Guid TakecareComboServiceId { get; set; }

        public string Status { get; set; } = null!;
    }
    public class TakecareComboServiceAssignTechModel
    {
        public Guid TakecareComboServiceId { get; set; }

        public Guid TechnicianID { get; set; }
    }
    public class TakecareComboServiceUpdateModel
    {
        public Guid Id { get; set; }

        public Guid? TakecareComboId { get; set; }

        public string? StartDate { get; set; }

        public int? NumOfMonth { get; set; }

        public bool? IsAtShop { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public int? TreeQuantity { get; set; }

    }
    public class TakecareComboServiceDetail
    {
        public string? TakecareComboName { get; set; }

        public double? TakecareComboPrice { get; set; }

        public string? TakecareComboDescription { get; set; }

        public string? TakecareComboGuarantee { get; set; }
    }
}

