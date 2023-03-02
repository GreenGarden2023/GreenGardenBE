﻿namespace GreeenGarden.Data.Models.TransactionModel
{
    public class TransactionModel
    {
    }
    public class TransactionRequestFirstModel
    {
        public Guid AddendumId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Type { get; set; }
        public double? NumberMoney { get; set; }
    }
    public class TransactionRequestModel
    {
        public Guid PaymentId { get; set; }
        public double? NumberMoney { get; set; }
    }
}
