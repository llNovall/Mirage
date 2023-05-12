using Azure;
using Domain.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using WebApp.Models.Blog;
using WebApp.Utils;

namespace WebApp.Controllers.Blog
{
    public class BlogController : Controller
    {
        private readonly IDBService _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<BlogController> _logger;
        private readonly EventId _eventId;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IAuthorizationService _authorizationService;

        public BlogController(IDBService dBService,
            UserManager<IdentityUser> userManager,
            ILogger<BlogController> logger,
            IUserStore<IdentityUser> userStore,
            IAzureStorageService azureStorageService,
            IAuthorizationService authorizationService)
        {
            _db = dBService;
            _logger = logger;
            _eventId = new EventId(200, name: "PostController");
            _userManager = userManager;
            _azureStorageService = azureStorageService;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index(string? blogPostId)
        {
            if (string.IsNullOrWhiteSpace(blogPostId))
            {
                _logger.LogError("BLOG INDEX VIEW FAILED", "BlogPostId is null or empty.");
                return RedirectToRoute("default");
            }

            BlogIndexModel model = new();

            BlogPost? blogPost = await _db.PostRepository.FindByIdAsync(blogPostId);

            if (blogPost != null)
            {
                IList<Comment> comments = await _db.CommentRepository.GetAllAsync(Guid.Parse(blogPostId));

                model.Comments = comments;
                model.BlogPost = blogPost;

                model.CommentCreate = new CommentCreateModel
                {
                    BlogId = blogPost.Id
                };
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            _logger.LogDebug(_eventId, "Displaying view to create post.");

            BlogCreateModel model = new();

            List<Tag> tags = (List<Tag>)await _db.TagRepository.GetAllAsync();

            if (tags.Count <= 0)
            {
                _logger.LogError("BLOG CREATE VIEW FAILED", "Tags from database returned empty.");
                return RedirectToRoute("default");
            }

            tags.ForEach(c => model.TagsList.Add(new TagCheckItem { TagId = c.Id.ToString(), TagName = c.TagName, IsChecked = false }));

            return View(model);
        }

        [Authorize]
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

                if (tags.Count <= 0)
                {
                    _logger.LogError("BLOG CREATE FAILED", "Tags from database returned empty.");
                    return View(model);
                }

                string? userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("BLOG CREATE FAILED", "Failed to find user id.");
                    return View(model);
                }

                Author? author = await _db.AuthorRepository.FindByIdAsync(userId);

                if (author == null)
                {
                    _logger.LogError("BLOG CREATE FAILED", $"Failed to find author with id - {userId}.");
                    return View(model);
                }

                List<string> tagIds = checkedTags.Select(c => c.TagId).ToList();

                List<Tag> selectedTags = tags.Where(c => tagIds.Contains(c.Id)).ToList();

                BlogPost blogPost = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = model.Title,
                    Author = author,
                    BodyContent = model.BodyContent,
                    Tags = selectedTags,
                };

                int result = await _db.PostRepository.AddAsync(blogPost);

                if (result <= 0)
                {
                    _logger.LogError("BLOG CREATE FAILED", $"Failed to create blog by user - {userId}.");
                    return View(model);
                }

                return RedirectToAction("Index", new { blogPostId = blogPost.Id.ToString() });
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> EditAsync(string? blogPostId)
        {
            BlogEditModel model = new();

            if (string.IsNullOrEmpty(blogPostId))
            {
                _logger.LogError("BLOG EDIT VIEW FAILED", "BlogPostId is null or empty.");
                return RedirectToRoute("default");
            }

            BlogPost? blogPost = await _db.PostRepository.FindByIdAsync(blogPostId);

            if (blogPost == null)
            {
                _logger.LogError("BLOG EDIT VIEW FAILED", $"Failed to retrieve blog with id - {blogPostId}.");
                return RedirectToRoute("default");
            }

            var result = await _authorizationService.AuthorizeAsync(User, blogPost, "BlogEdit");

            if (!result.Succeeded)
            {
                _logger.LogError("BLOG EDIT VIEW FAILED", $"Failed to authorize user for blog with id - {blogPostId}.");
                return Forbid();
            }

            List<Tag> tags = (List<Tag>)await _db.TagRepository.GetAllAsync();

            if (tags.Count <= 0)
            {
                _logger.LogError("BLOG EDIT VIEW FAILED", "Tags from database returned empty.");
                return RedirectToRoute("default");
            }

            model.BlogId = blogPostId;
            model.Title = blogPost.Title;
            model.BodyContent = blogPost.BodyContent;

            tags.ForEach(c =>
                {
                    TagCheckItem tagCheckItem = new()
                    {
                        TagName = c.TagName,
                        IsChecked = blogPost.Tags.Contains(c),
                        TagId = c.Id
                    };

                    model.TagsList.Add(tagCheckItem);
                }
            );

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(BlogEditModel model)
        {
            if (ModelState.IsValid)
            {
                string? userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning(_eventId, "BLOG EDIT FAILED", $"Failed to find userId to edit blog with id - {model.BlogId}.");
                    return View(model);
                }

                BlogPost? blogFromDb = await _db.PostRepository.FindByIdAsync(model.BlogId);

                if (blogFromDb == null)
                {
                    _logger.LogCritical(_eventId, "BLOG EDIT FAILED", $"Failed to find blog - {model.BlogId}.");
                    return View(model);
                }

                var authorizationResult = await _authorizationService.AuthorizeAsync(User, blogFromDb, "BlogEdit");

                if (!authorizationResult.Succeeded)
                {
                    _logger.LogWarning(_eventId, "BLOG EDIT FAILED", $"{userId} attempted to update comment {model.BlogId} without proper authorization.");
                    return Forbid();
                }

                List<TagCheckItem> checkedTags = model.TagsList.Where(tag => tag.IsChecked).ToList();

                IList<Tag> tags = await _db.TagRepository.GetAllAsync();

                if (tags.Count <= 0)
                {
                    _logger.LogError("BLOG EDIT FAILED", "Tags from database returned empty.");
                    return View(model);
                }

                List<string> tagIds = checkedTags.Select(c => c.TagId).ToList();

                List<Tag> selectedTags = tags.Where(c => tagIds.Contains(c.Id)).ToList();

                blogFromDb.Title = model.Title;
                blogFromDb.BodyContent = model.BodyContent;
                blogFromDb.Tags = selectedTags;

                int result = await _db.PostRepository.UpdateAsync(blogFromDb);

                if (result <= 0)
                    _logger.LogCritical(_eventId, "BLOG EDIT FAILED", $"Database failed to update blog - {model.BlogId}.");
                else
                    _logger.LogInformation(_eventId, "BLOG EDIT SUCCESS", $"Successfully updated blog - {model.BlogId}.");

                return RedirectToAction("Index", new { blogPostId = blogFromDb.Id });
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> PostImage(IFormCollection formCollection)
        {
            Hashtable locations = new();

            if (formCollection.Files.Count <= 0)
                return Json(locations);

            using (Stream stream = formCollection.Files[0].OpenReadStream())
            {
                string imgType = formCollection.Files[0].ContentType;
                var location = await _azureStorageService.UploadImageAsync(imgType, stream);
                locations.Add("location", location);
            }

            return Json(locations);
        }
    }
}