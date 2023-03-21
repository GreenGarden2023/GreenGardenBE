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
    }
}
