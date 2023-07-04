using Domain.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using WebApp.Models.Admin;
using static WebApp.Models.Admin.AdminManageTagsModel;

namespace WebApp.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IDBService _dbService;
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(IDBService dbService, ILogger<AdminController> logger, UserManager<IdentityUser> userManager)
        {
            _dbService = dbService;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult AdminView()
        {
            return View();
        }

        public async Task<IActionResult> ManageTags()
        {
            List<Tag> tags = (List<Tag>)await _dbService.TagRepository.GetAllAsync();

            AdminManageTagsModel model = new(tags);

            return View(model);
        }

        public async Task<IActionResult> EditTag(string tagId)
        {
            Tag? tag = await _dbService.TagRepository.FindByIdAsync(tagId);

            if (tag == null)
            {
                _logger.LogWarning("TAG EDIT VIEW FAILED", $"Failed to find tag with id {tagId}.");
                return RedirectToAction("ManageTags");
            }

            AdminEditTagModel adminEditTagModel = new(tag);

            return View(adminEditTagModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTag(AdminEditTagModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.TagId) || string.IsNullOrEmpty(model.TagName) || string.IsNullOrEmpty(model.TagDescription))
                {
                    _logger.LogWarning("TAG EDIT FAILED", $"Model failed validation");
                    return RedirectToAction("ManageTags");
                }

                Tag? tag = await _dbService.TagRepository.FindByIdAsync(model.TagId);

                if (tag == null)
                {
                    _logger.LogWarning("TAG EDIT FAILED", $"Failed to find tag with id {model.TagId}.");
                    return RedirectToAction("ManageTags");
                }

                tag.TagDescription = model.TagDescription;

                int result = await _dbService.TagRepository.UpdateAsync(tag);

                if (result == 0)
                {
                    _logger.LogWarning("TAG EDIT FAILED", $"Failed to edit tag with id {model.TagId} due to database error.");
                }

                return RedirectToAction("ManageTags");
            }

            return View(model);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTag(string tagId)
        {
            if (string.IsNullOrEmpty(tagId))
            {
                _logger.LogWarning("TAG DELETE FAILED", $"Failed to delete tag {tagId}.");
                return RedirectToAction("AdminView");
            }

            Tag? tag = await _dbService.TagRepository.FindByIdAsync(tagId);

            if (tag == null)
            {
                _logger.LogWarning("TAG DELETE FAILED", $"Failed to find tag with id {tagId}.");
                return RedirectToAction("AdminView");
            }

            int result = await _dbService.TagRepository.RemoveAsync(tag);

            if (result == 0)
            {
                _logger.LogWarning("TAG DELETE FAILED", $"Failed to delete tag with id {tagId} due to database error.");
                return RedirectToAction("AdminView");
            }

            return RedirectToAction("AdminView");
        }

        public IActionResult CreateTag()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTag(AdminCreateTagModel model)
        {
            if (ModelState.IsValid)
            {
                Tag? tag = await _dbService.TagRepository.FindAsync(c => c.TagName == model.TagName);

                if (tag != null)
                {
                    ModelState.AddModelError("Duplicate Tag", "There is already a tag with this name.");
                    return View(model);
                }

                IdentityUser? identityUser = await _userManager.GetUserAsync(User);

                if (identityUser == null)
                {
                    ModelState.AddModelError("Error", "Something went wrong. Please try again later.");
                    return View(model);
                }

                Author? author = await _dbService.AuthorRepository.FindByIdAsync(identityUser.Id);

                if (author == null)
                {
                    _logger.LogCritical("TAG CREATE FAILED", "Failed to find author.");
                    return RedirectToAction("ManageTags");
                }

                tag = new Tag
                {
                    Id = Guid.NewGuid().ToString(),
                    Author = author,
                    TagName = model.TagName,
                    TagDescription = model.TagDescription
                };

                int result = await _dbService.TagRepository.AddAsync(tag);

                if (result == 0)
                {
                    _logger.LogCritical("TAG CREATE FAILED", "Failed to find author.");
                    return View(model);
                }

                return RedirectToAction("ManageTags");
            }

            return View(model);
        }
    }
}