namespace GreeenGarden.Data.Enums
{
    public class TransactionType
    {
        /* Các khoản thanh toán: 
         *  - Tiền cọc
         *  - Trả phụ lục(addendum) khi thuê
         *  - Trả đơn hàng(order) khi mua
         */

        public static readonly string RECEIVED = "received";
        public static readonly string DEPOSIT = "deposit";//Khoản thanh toán thuộc về tiền cọc


    }
}
