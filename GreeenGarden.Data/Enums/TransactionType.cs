namespace GreeenGarden.Data.Enums
{
    public class TransactionStatus
    {
        public static readonly string RECEIVED = "received";
        public static readonly string DEPOSIT = "refund";
    }
    public class TransactionType
    {
        public static readonly string RENT_DEPOSIT = "rent deposit";
        public static readonly string SALE_DEPOSIT = "sale deposit";
        public static readonly string SERVICE_DEPOSIT = "service deposit";

        public static readonly string RENT_ORDER = "rent payment";
        public static readonly string SALE_ORDER = "sale payment";
        public static readonly string SERVICE_ORDER = "service payment";
    }
}
