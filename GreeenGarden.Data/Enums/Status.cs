﻿namespace GreeenGarden.Data.Enums
{
    public class Status
    {
        public static readonly string ACTIVE = "active";//Item: 
        public static readonly string DISABLE = "disable";//Item:
        public static readonly string UNPAID = "unpaid"; //Order, addnedum: Đơn hàng vừa được tạo
        public static readonly string READY = "ready"; //Order, addendum: Đơn hàng đã thanh toán cọc
        public static readonly string PAID = "paid"; //Order, addendum:Rent: Đã thanh toán đủ
        public static readonly string COMPLETED = "completed"; //Order, addendum: Đã hoàn cọc, order kết thúc hoặc thêm 1 addendum mới, sale: thanh toán đủ
        public static readonly string CANCEL = "cancel"; //Order, addendum: bị hủy
        public static readonly string DELIVERY = "delivery"; //Order: Dăng vận chuyển
        public static readonly string RENTING = "renting"; //Order: Dăng vận chuyển

    }
    public class TreeStatus
    {
        public static readonly string ACTIVE = "active";
        public static readonly string DISABLE = "disable";
    }

    public class ServiceStatus
    {
        public static readonly string PROCESSING = "processing";
        public static readonly string ACCEPTED = "accepted";
        public static readonly string REJECTED = "rejected";
        public static readonly string USER_APPROVED = "user approved";
        public static readonly string CONFIRMED = "confirmed";
        public static readonly string REPROCESS = "reprocess";
        public static readonly string COMPLETED = "completed";
        public static readonly string CANCEL = "cancel";
        public static readonly string TAKINGCARE = "taking care";
    }

    public class ServiceOrderStatus
    {
        public static readonly string UNPAID = "unpaid"; //Order, addnedum: Đơn hàng vừa được tạo
        public static readonly string READY = "ready"; //Order, addendum: Đơn hàng đã thanh toán cọc
        public static readonly string PAID = "paid"; //Order, addendum:Rent: Đã thanh toán đủ
        public static readonly string COMPLETED = "completed"; //Order, addendum: Đã hoàn cọc, order kết thúc hoặc thêm 1 addendum mới, sale: thanh toán đủ
        public static readonly string CANCEL = "cancel"; //Order, addendum: bị hủy

    }
    public class ServiceCalendarStatus
    {
        public static readonly string PENDING = "pending";
        public static readonly string DONE = "done";

    }
    public class UserStatus
    {
        public static readonly string ENABLE = "enable";
        public static readonly string DISABLED = "disabled"; 
    }
    public class TakecareComboServiceStatus
    {
        public static readonly string ACCEPTED = "accepted";
        public static readonly string REJECTED = "rejected";
        public static readonly string PENDING = "pending";
        public static readonly string TAKINGCARE = "taking care";
        public static readonly string REPROCESS = "reprocess";
        public static readonly string CANCEL = "cancel";

    }
}
