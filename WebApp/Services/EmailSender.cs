using Microsoft.AspNetCore.Identity.UI.Services;

namespace WebApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmailSender(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}