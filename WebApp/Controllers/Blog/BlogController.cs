using Domain.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Blog;

namespace WebApp.Controllers.Blog
{
    public class BlogController : Controller
    {
        private readonly IDBService _db;
        private readonly ILogger<BlogController> _logger;
        private readonly EventId _eventId;

        public BlogController(IDBService dBService, ILogger<BlogController> logger)
        {
            _db = dBService;
            _logger = logger;
            _eventId = new EventId(200, name: "PostController");
        }

        public async Task<IActionResult> Index(string blogPostId)
        {
            BlogPost? blogPost = await _db.PostRepository.FindByIdIncludeNavigationAsync(blogPostId);

            return View(blogPost);
        }

        public IActionResult Create()
        {
            _logger.LogDebug(_eventId, "Displaying view to create post.");
            BlogCreateModel model = new();
            List<Tag> tags = _db.TagRepository.GetAll().ToList();

            foreach (Tag tag in tags)
            {
                model.TagsList.Add(new TagCheckItem { TagId = tag.Id.ToString(), TagName = tag.TagName, IsChecked = false });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(BlogCreateModel model)
        {
            if (model.TagsList.Count(c => c.IsChecked == true) == 0)
            {
                ModelState.AddModelError(string.Empty, "Must select at least 1 tag.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                List<TagCheckItem> checkedTags = model.TagsList.Where(tag => tag.IsChecked).ToList();
                List<Tag> tags = _db.TagRepository.GetAll().ToList();

                List<Guid> tagIds = new();

                checkedTags.Select(c => c.TagId).ToList().ForEach(c => tagIds.Add(Guid.Parse(c)));

                List<Tag> selectedTags = tags.Where(c => tagIds.Contains(c.Id)).ToList();

                BlogPost blogPost = new()
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    AuthorId = Guid.NewGuid(),
                    BodyContent = model.BodyContent,
                    Tags = selectedTags,
                };

                int result = await _db.PostRepository.AddAsync(blogPost);

                if (result > 0)
                {
                    return RedirectToAction("Index", new { blogPostId = blogPost.Id.ToString() });
                }
            }
            return View(model);
        }
    }
}