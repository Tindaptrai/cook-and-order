using System.Net;
using System.Net.Mail;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DACS_Food.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var host = _configuration["Smtp:Host"];
            var userName = _configuration["Smtp:UserName"];
            var password = _configuration["Smtp:Password"];
            var fromEmail = _configuration["Smtp:FromEmail"];
            var portText = _configuration["Smtp:Port"];

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(userName)
                || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fromEmail))
            {
                return;
            }

            var port = int.TryParse(portText, out var parsedPort) ? parsedPort : 587;
            var fromName = _configuration["Smtp:FromName"] ?? "FoodieTTTM";
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(userName, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

