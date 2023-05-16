using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Contact;

namespace WebApp.Controllers
{
    public class ContactController : Controller
    {
        private readonly IEmailSender _emailSender;

        public ContactController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [RequireHttps]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [RequireHttps]
        [AllowAnonymous]
        public async Task<IActionResult> Send(ContactInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _emailSender.SendEmailAsync(model.EmailAddress, model.Subject, model.Message);
                }
                catch (Exception ex)
                {
                }

                return View(model);
            }

            return View("Index", model);
        }
    }
}