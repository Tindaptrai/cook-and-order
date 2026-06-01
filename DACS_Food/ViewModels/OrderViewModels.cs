using DACS_Food.Models;
using System.ComponentModel.DataAnnotations;

namespace DACS_Food.ViewModels
{
    public class CheckoutViewModel
    {
        public CartViewModel Cart { get; set; } = new();
        public IReadOnlyList<RestaurantTable> Tables { get; set; } = Array.Empty<RestaurantTable>();
        public string? DiscountCode { get; set; }
    }

    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? CustomerEmail { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
        public string Address { get; set; } = string.Empty;
        public OrderType OrderType { get; set; } = OrderType.Delivery;
        public int? TableId { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
        public string? DiscountCode { get; set; }
        public string? CartJson { get; set; }
    }

    public class OrderTrackingViewModel
    {
        public string? OrderCode { get; set; }
        public string? Phone { get; set; }
        public string? Message { get; set; }
        public bool IsAccountLookup { get; set; }
        public IReadOnlyList<Order> Orders { get; set; } = Array.Empty<Order>();
    }
    public class QrPaymentInfoViewModel
    {
        public string QrImageUrl { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string BankBin { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public string BankAccountName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransferContent { get; set; } = string.Empty;
    }
}
