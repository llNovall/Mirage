using Domain.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WebApp.Models.Blog;

namespace WebApp.Controllers.Blog
{
    public class BlogController : Controller
    {
        private readonly IDBService _db;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<BlogController> _logger;
        private readonly EventId _eventId;

        public BlogController(IDBService dBService, ILogger<BlogController> logger, IUserStore<IdentityUser> userStore)
        {
            _db = dBService;
            _logger = logger;
            _eventId = new EventId(200, name: "PostController");
            _userStore = userStore;
        }

        public async Task<IActionResult> Index(string blogPostId)
        {
            BlogIndexModel model = new BlogIndexModel();
            BlogPost? blogPost = await _db.PostRepository.FindByIdAsync(blogPostId);
            IList<Comment> comments = await _db.CommentRepository.GetAllAsync(Guid.Parse(blogPostId));

            model.Comments = comments;
            model.BlogPost = blogPost;

            if (blogPost != null)
            {
                model.CommentCreate = new CommentCreateModel
                {
                    BlogId = blogPost.Id
                };
            }

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            _logger.LogDebug(_eventId, "Displaying view to create post.");
            BlogCreateModel model = new();
            IList<Tag> tags = await _db.TagRepository.GetAllAsync();

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
                IList<Tag> tags = await _db.TagRepository.GetAllAsync();

                List<string> tagIds = new();

                checkedTags.Select(c => c.TagId).ToList().ForEach(c => tagIds.Add(c));

                List<Tag> selectedTags = tags.Where(c => tagIds.Contains(c.Id)).ToList();

                BlogPost blogPost = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = model.Title,
                    AuthorId = Guid.NewGuid().ToString(),
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