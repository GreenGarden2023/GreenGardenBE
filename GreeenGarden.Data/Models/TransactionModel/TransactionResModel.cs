namespace GreeenGarden.Data.Models.TransactionModel
{
    public class TransactionResModel
    {
        public Guid Id { get; set; }

        public Guid OrderID { get; set; }

        public double Amount { get; set; }

        public DateTime PaidDate { get; set; }

        public string? Type { get; set; }

        public string? Status { get; set; }

        public string? Description { get; set; }

        public PaymentType? PaymentType { get; set; }
    }

    public class PaymentType
    {
        public Guid Id { get; set; }

        public string? PaymentName { get; set; }
    }
}

