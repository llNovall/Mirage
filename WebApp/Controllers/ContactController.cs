using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Contact;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class ContactController : Controller
    {
        private readonly IEmailService _emailService;
        private string? _contactEmail;

        public ContactController(IEmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _contactEmail = configuration.GetSection("contactEmail").Get<string>();
        }

        [RequireHttps]
        public IActionResult Index()
        {
            ContactInputModel model = new();
            model.EmailSendStatus = EmailSendStatus.Pending;
            return View(model);
        }

        [HttpPost]
        [RequireHttps]
        [AllowAnonymous]
        public async Task<IActionResult> Send(ContactInputModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(_contactEmail))
                    return View("Index", model);

                bool isSent = await _emailService.SendEmailAsync(_contactEmail, model.EmailAddress, $"{model.Name}", model.Subject, model.Message);

                if (isSent)
                    model.EmailSendStatus = EmailSendStatus.Success;
                else
                    model.EmailSendStatus = EmailSendStatus.Failed;

                return View("Index", model);
            }

            return View("Index", model);
        }
    }
}