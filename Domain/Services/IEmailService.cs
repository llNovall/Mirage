using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string fromEmail, string displayName, string subject, string htmlMessage);
    }
}