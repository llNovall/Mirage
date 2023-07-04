using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult AdminView()
        {
            return View();
        }
    }
}