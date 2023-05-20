using System;
using GreeenGarden.Data.Models.TakecareComboServiceModel;

namespace GreeenGarden.Data.Models.TakecareComboOrder
{
    public class TakecareComboOrderModel
    {
        public Guid Id { get; set; }

        public string OrderCode { get; set; } = null!;

        public DateTime CreateDate { get; set; }

        public DateTime ServiceStartDate { get; set; }

        public DateTime ServiceEndDate { get; set; }

        public double? Deposit { get; set; }

        public double TotalPrice { get; set; }

        public double RemainAmount { get; set; }

        public Guid? TechnicianId { get; set; }

        public Guid? UserId { get; set; }

        public string Status { get; set; } = null!;

        public string? Description { get; set; }

        public Guid? CancelBy { get; set; }

        public TakecareComboServiceViewModel? TakecareComboService { get; set; }
    }

    public class TakecareComboOrderCreateModel
    {
        public Guid TakecareComboServiceId { get; set; }
    }
    public class GetTakecareComboOrderResModel
    {
        public object? Paging { get; set; }
        public object? TakecareComboOrderList { get; set; }
    }
    public class TakecareComboOrderUpdateStatusModel
    {
        public Guid TakecareComboOrderId { get; set; }
        public string Status { get; set; }
    }
    public class TakecareComboOrderCancelModel
    {
        public Guid TakecareComboOrderId { get; set; }
        public string CancelReason { get; set; }
    }
}

