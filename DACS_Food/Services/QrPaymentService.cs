using System.Globalization;
using System.Net;
using DACS_Food.Models;
using DACS_Food.ViewModels;

namespace DACS_Food.Services
{
    public class QrPaymentService : IQrPaymentService
    {
        private readonly IConfiguration _configuration;

        public QrPaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public QrPaymentInfoViewModel CreateQrInfo(Order order)
        {
            var bankBin = _configuration["QrPayment:BankBin"] ?? string.Empty;
            var accountNumber = _configuration["QrPayment:BankAccountNumber"] ?? string.Empty;
            var template = _configuration["QrPayment:Template"] ?? "compact2";
            var accountName = _configuration["QrPayment:BankAccountName"] ?? string.Empty;
            var amount = Math.Round(order.TotalAmount, 0, MidpointRounding.AwayFromZero);
            var prefix = _configuration["QrPayment:DescriptionPrefix"] ?? "FOODIELAB";
            var transferContent = $"{prefix} DH{order.Id}";

            var amountText = amount.ToString("0", CultureInfo.InvariantCulture);
            var qrImageUrl = $"https://img.vietqr.io/image/{WebUtility.UrlEncode(bankBin)}-{WebUtility.UrlEncode(accountNumber)}-{WebUtility.UrlEncode(template)}.png" +
                $"?amount={WebUtility.UrlEncode(amountText)}" +
                $"&addInfo={WebUtility.UrlEncode(transferContent)}" +
                $"&accountName={WebUtility.UrlEncode(accountName)}";

            return new QrPaymentInfoViewModel
            {
                QrImageUrl = qrImageUrl,
                BankName = _configuration["QrPayment:BankName"] ?? string.Empty,
                BankCode = _configuration["QrPayment:BankCode"] ?? string.Empty,
                BankBin = bankBin,
                BankAccountNumber = accountNumber,
                BankAccountName = accountName,
                Amount = amount,
                TransferContent = transferContent
            };
        }
    }
}
