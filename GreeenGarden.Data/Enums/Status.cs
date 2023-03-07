namespace GreeenGarden.Data.Enums
{
    public class Status
    {
        public static readonly string ACTIVE = "active";//Item: 
        public static readonly string DISABLE = "disable";//Item:
        public static readonly string UNPAID = "unpaid"; //Order, addnedum: Đơn hàng vừa được tạo
        public static readonly string READY = "ready"; //Order, addendum: Đơn hàng đã thanh toán cọc
        public static readonly string PAID = "paid"; //Order, addendum: Đã thanh toán đủ
        public static readonly string COMPLETED = "completed"; //Order: Đã hoàn cọc, order kết thúc hoặc thêm 1 addendum mới
        public static readonly string CANCEL = "cancel"; //Order: bị hủy
    }
}
