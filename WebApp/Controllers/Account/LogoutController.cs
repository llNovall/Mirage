using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers.Account
{
    public class LogoutController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(SignInManager<IdentityUser> signInManager, ILogger<LogoutController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            return RedirectToAction("Index", "Home");
        }
    }
}