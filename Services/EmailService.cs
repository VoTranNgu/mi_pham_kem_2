using System.Net;
using System.Net.Mail;


namespace mi_pham_kem.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var smtpHost = _config["SmtpSettings:Host"];
            var smtpPort = int.Parse(_config["SmtpSettings:Port"]);
            var smtpUser = _config["SmtpSettings:Email"];
            var smtpPass = _config["SmtpSettings:Password"];
            var enableSSL = bool.Parse(_config["SmtpSettings:EnableSSL"]);

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = enableSSL
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser, "Cửa hàng mĩ phẩm Chella"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            client.Send(mailMessage);
        }
    }
}
