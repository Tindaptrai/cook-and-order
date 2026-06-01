using DACS_Food.Data;
using DACS_Food.Models;
using Microsoft.EntityFrameworkCore;

namespace DACS_Food.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly ILoyaltyService _loyaltyService;
        private readonly IQrPaymentService _qrPaymentService;

        public PaymentService(ApplicationDbContext db, IConfiguration configuration, ILoyaltyService loyaltyService, IQrPaymentService qrPaymentService)
        {
            _db = db;
            _configuration = configuration;
            _loyaltyService = loyaltyService;
            _qrPaymentService = qrPaymentService;
        }

        public async Task<Payment> CreatePaymentAsync(Order order)
        {
            order.PaymentStatus = PaymentStatus.Pending;
            var qrInfo = _qrPaymentService.CreateQrInfo(order);

            var payment = new Payment
            {
                OrderId = order.Id,
                Method = order.PaymentMethod,
                Amount = order.TotalAmount,
                Status = PaymentStatus.Pending,
                BankName = qrInfo.BankName,
                BankAccountNumber = qrInfo.BankAccountNumber,
                BankAccountName = qrInfo.BankAccountName,
                QrContent = qrInfo.TransferContent
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            order.Payment = payment;
            return payment;
        }

        public async Task<bool> ConfirmDemoPaymentAsync(string orderCode)
        {
            var order = await _db.Orders
                .Include(x => x.Payment)
                .FirstOrDefaultAsync(x => x.OrderCode == orderCode);

            if (order?.Payment == null) return false;

            order.PaymentStatus = PaymentStatus.Paid;
            order.Payment.Status = PaymentStatus.Paid;
            order.Payment.PaidAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            await _loyaltyService.RefreshUserLevelAsync(order.UserId);
            return true;
        }
    }
}
