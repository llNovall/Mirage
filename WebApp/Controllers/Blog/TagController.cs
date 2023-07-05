using Domain.Entities;
using Domain.Services;
using EFDataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;
using WebApp.Models.Blog;
using WebApp.Models.Home;

namespace WebApp.Controllers.Blog
{
    public class TagController : Controller
    {
        private readonly IDBService _dbService;
        private readonly ILogger<TagController> _logger;
        private readonly EventId _eventId;

        public TagController(IDBService dBService, ILogger<TagController> logger)
        {
            _dbService = dBService;
            _logger = logger;
            _eventId = new EventId(202, "TagController");
        }

        public async Task<IActionResult> Index(string tagId, int currentPage = 1)
        {
            if (string.IsNullOrEmpty(tagId))
            {
                _logger.LogWarning(_eventId, "TAG INDEX VIEW FAILED", $"TagId is missing.");
                return RedirectToAction("Index", "Home");
            }

            Tag? tag = await _dbService.TagRepository.FindByIdAsync(tagId);

            if (tag == null)
            {
                _logger.LogWarning(_eventId, "TAG INDEX VIEW FAILED", $"Failed to find tag.");
                return RedirectToAction("Index", "Home");
            }

            IList<BlogPost> blogPosts = await _dbService.TagRepository.GetPostsByTagIdAsync(tagId);

            int numOfPost = blogPosts.Count;
            int startIndex = (currentPage - 1) * 10;
            int endIndex = ((currentPage - 1) * 10) + 10;

            TagIndexViewModel model = new(
                tagId: tagId,
                tagName: tag.TagName,
                currentPage: currentPage,
                numOfPages: numOfPost == 0 ? 1 : (numOfPost / 10) + 1,
                blogPosts: blogPosts.ToList().Take(new Range(startIndex, endIndex)).ToList(),
                dictPostedOn: await _dbService.PostRepository.GetDictionaryOfPostedDateAsync(),
                tagBlogPostCountDatas: await _dbService.TagRepository.GetTagsPostsCountDataList()
            );

            _logger.LogInformation(_eventId, "TAG INDEX VIEW SUCCESS", "Successfully loaded model and passed to view.");

            return View(model);
        }
    }
}