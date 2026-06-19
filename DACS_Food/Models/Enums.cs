namespace DACS_Food.Models
{
    public enum PaymentMethod
    {
        COD = 1,
        QR = 2
    }

    public enum PaymentStatus
    {
        Unpaid = 1,
        Pending = 2,
        Paid = 3,
        Failed = 4
    }

    public enum OrderStatus
    {
        Pending = 1,
        New = 1,
        Confirmed = 2,
        Preparing = 3,
        ReadyForDelivery = 6,
        Delivering = 7,
        Delivered = 8,
        Completed = 4,
        Cancelled = 5
    }

    public enum DeliveryStatus
    {
        Pending = 1,
        ReadyForDelivery = 2,
        AssigningShipper = 2,
        Delivering = 3,
        Shipping = 3,
        Delivered = 4,
        Failed = 5,
        Cancelled = 6
    }

    public enum OrderType
    {
        Delivery = 1,
        DineIn = 2,
        TakeAway = 3
    }

    public enum TableType
    {
        Normal = 1,
        Private = 2
    }

    public enum TableStatus
    {
        Available = 1,
        Occupied = 2,
        Reserved = 3,
        Maintenance = 4
    }

    public enum ReservationStatus
    {
        Pending = 1,
        Confirmed = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum DiscountType
    {
        Percent = 1,
        FixedAmount = 2
    }

    public enum DiscountScope
    {
        Manual = 1,
        GoldenHour = 2,
        Loyalty = 3
    }

    public enum OtpPurpose
    {
        Register = 1,
        Login = 2,
        ForgotPassword = 3,
        ChangePassword = 4
    }
}
