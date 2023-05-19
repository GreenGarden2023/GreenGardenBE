namespace GreeenGarden.Data.Models.MoMoModel
{
    public class MoMoOrderModel
    {
        public Guid OrderId { get; set; }
        public double PayAmount { get; set; }
        public string? OrderType { get; set; }
    }
    public class MoMoDepositModel
    {
        public Guid OrderId { get; set; }
        public string? OrderType { get; set; }
    }
    public class MoMoPaymentModel
    {
        public Guid OrderId { get; set; }
        public double Amount { get; set; }
        public string? OrderType { get; set; }
        public string? PaymentType { get; set; }
    }
    public class TakecareComboOrderDepositPaymentModel
    {
        public Guid OrderId { get; set; }
        public string PaymentMethod { get; set; }
    }
    public class TakecareComboOrderPaymentModel
    {
        public Guid OrderId { get; set; }
        public double? Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentType { get; set; }
    }
    public class MoMoTakecareComboOrderPaymentModel
    {
        public Guid OrderId { get; set; }
        public double PayAmount { get; set; }
    }
}

