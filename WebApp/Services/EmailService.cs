using Domain.Services;
using System.Net.Mail;
using System.Net;

namespace WebApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient? _smtpClient;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _logger = logger;

            string? smtpAddress = configuration.GetSection("smtpAddress").Get<string>();
            int? smtpPort = configuration.GetSection("smtpPort").Get<int>();
            string? smtpUserName = configuration.GetSection("smtpUserName").Get<string>();
            string? smtpPassword = configuration.GetSection("smtpPassword").Get<string>();

            if (!string.IsNullOrEmpty(smtpAddress)
                && smtpPort != null
                && !string.IsNullOrEmpty(smtpUserName)
                && !string.IsNullOrEmpty(smtpPassword))
            {
                NetworkCredential netCre = new(smtpUserName, smtpPassword);

                _smtpClient = new SmtpClient(smtpAddress, (int)smtpPort)
                {
                    EnableSsl = true,
                    Credentials = netCre
                };
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string fromEmail, string displayName, string subject, string htmlMessage)
        {
            if (_smtpClient == null)
            {
                _logger.LogError("SMTP NOT INITIALIZED");
                return false;
            }

            MailMessage mail = new MailMessage();
            mail.Subject = subject;
            mail.From = new MailAddress(fromEmail, displayName, System.Text.Encoding.UTF8);
            mail.To.Add(toEmail);
            mail.Body = htmlMessage;
            mail.IsBodyHtml = true;

            await _smtpClient.SendMailAsync(mail);
            _logger.LogInformation("Successfully sent email.");

            return true;
        }
    }
}