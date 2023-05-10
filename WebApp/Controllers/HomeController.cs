using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApp.Models;
using WebApp.Models.Home;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDBService _dbService;

        public HomeController(ILogger<HomeController> logger, IDBService dBService)
        {
            _logger = logger;
            _dbService = dBService;
        }

        public async Task<IActionResult> Index(int currentPage = 1)
        {
            int numOfPost = await _dbService.PostRepository.GetBlogPostCountAsync();
            HomeIndexViewModel model = new();
            model.NumOfPages = (numOfPost / 10);
            model.BlogPosts = await _dbService.PostRepository.GetBlogPostsAsync(currentPage, 10);
            model.DictPostedOn = await _dbService.PostRepository.GetDictionaryOfPostedDateAsync();
            model.CurrentPage = currentPage;
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}