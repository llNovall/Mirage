using Domain.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Blog;

namespace WebApp.Controllers.Blog
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly IDBService _db;
        private readonly ILogger<CommentController> _logger;
        private readonly EventId _eventId;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public CommentController(IDBService dBService, ILogger<CommentController> logger, UserManager<IdentityUser> userManager, IAuthorizationService authorizationService)
        {
            _db = dBService;
            _logger = logger;
            _eventId = new EventId(201, name: "CommentController");
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PostComment(CommentCreateModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                string? userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning(_eventId, "COMMENT CREATE FAILED", $"Failed to find userId to create comment on blog post.");
                    return Redirection(returnUrl);
                }

                var authorizationResult = await _authorizationService.AuthorizeAsync(User, "CommentCreate");

                if (!authorizationResult.Succeeded)
                {
                    _logger.LogWarning(_eventId, "COMMENT CREATE FAILED", $"Attempted to create comment on blog post without proper authorization.");
                    return Redirection(returnUrl);
                }

                if (string.IsNullOrEmpty(model.BodyContent) | (model.BodyContent?.Length < 5 && model.BodyContent?.Length > 500))
                {
                    _logger.LogWarning(_eventId, "COMMENT REPLY FAILED", $"Comment is too short or long to create comment on blog post.");
                    return Redirection(returnUrl);
                }

                Author? author = await _db.AuthorRepository.FindByIdAsync(userId);

#pragma warning disable CS8601 // Possible null reference assignment.
                Comment comment = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Author = author,
                    BlogId = model.BlogId,
                    BodyContent = model.BodyContent,
                    PostedOn = DateTime.UtcNow,
                };
#pragma warning restore CS8601 // Possible null reference assignment.

                int result = await _db.CommentRepository.AddAsync(comment);

                if (result <= 0)
                    _logger.LogCritical(_eventId, "COMMENT CREATE FAILED", $"Database failed to create comment on blog post.");
                else
                    _logger.LogInformation(_eventId, "COMMENT CREATE SUCCESS", $"Successfully created comment on blog post.");
            }

            return Redirection(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditComment(CommentEditModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                string? userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning(_eventId, "COMMENT EDIT FAILED", $"Failed to find userId to edit comment.");
                    return Redirection(returnUrl);
                }

                Comment? commentFromDb = await _db.CommentRepository.FindByIdAsync(model.CommentId);

                if (commentFromDb == null)
                {
                    _logger.LogCritical(_eventId, "COMMENT EDIT FAILED", $"Failed to find comment.");
                    return Redirection(returnUrl);
                }

                var authorizationResult = await _authorizationService.AuthorizeAsync(User, commentFromDb, "CommentEdit");

                if (!authorizationResult.Succeeded)
                {
                    _logger.LogWarning(_eventId, "COMMENT EDIT FAILED", $"Attempted to update comment without proper authorization.");
                    return Redirection(returnUrl);
                }

                if (model.BodyContent.Length < 5 && model.BodyContent.Length > 500)
                {
                    _logger.LogWarning(_eventId, "COMMENT REPLY FAILED", $"Comment is too short or long to update comment.");
                    return Redirection(returnUrl);
                }

                if (commentFromDb.BodyContent != model.BodyContent)
                {
                    commentFromDb.BodyContent = model.BodyContent;

                    int result = await _db.CommentRepository.UpdateAsync(commentFromDb);

                    if (result <= 0)
                        _logger.LogCritical(_eventId, "COMMENT EDIT FAILED", $"Database failed to update.");
                    else
                        _logger.LogInformation(_eventId, "COMMENT EDIT SUCCESS", $"Successfully updated comment.");
                }
            }

            return Redirection(returnUrl);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(CommentDeleteModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                string? userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning(_eventId, "COMMENT DELETE FAILED", $"Failed to find userId to delete comment.");
                    return Redirection(returnUrl);
                }

                Comment? commentFromDb = await _db.CommentRepository.FindByIdAsync(model.CommentId);

                if (commentFromDb == null)
                {
                    _logger.LogCritical(_eventId, "COMMENT DELETE FAILED", $"Failed to find comment.");
                    return Redirection(returnUrl);
                }

                var authorizationResult = await _authorizationService.AuthorizeAsync(User, commentFromDb, "CommentDelete");
                var authorizationAdminResult = await _authorizationService.AuthorizeAsync(User, commentFromDb, "CommentDeleteAdmin");

                if (!authorizationResult.Succeeded & !authorizationAdminResult.Succeeded)
                {
                    _logger.LogWarning(_eventId, "COMMENT DELETE FAILED", $"{userId} attempted to delete comment without proper authorization.");
                    return Redirection(returnUrl);
                }

                int result = await _db.CommentRepository.RemoveAsync(commentFromDb);

                if (result == 0)
                    _logger.LogCritical(_eventId, "COMMENT DELETE FAILED", $"Database failed to delete comment.");
                else
                    _logger.LogInformation(_eventId, "COMMENT DELETE SUCCESS", $"Successfully deleted comment.");
            }

            return Redirection(returnUrl);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyComment(CommentReplyModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                string? userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning(_eventId, "COMMENT REPLY FAILED", $"Failed to find userId to reply to comment.");
                    return Redirection(returnUrl);
                }

                var authorizationResult = await _authorizationService.AuthorizeAsync(User, "CommentReply");

                if (!authorizationResult.Succeeded)
                {
                    _logger.LogWarning(_eventId, "COMMENT REPLY FAILED", $"{userId} attempted to reply to comment without proper authorization.");
                    return Redirection(returnUrl);
                }

                Author? author = await _db.AuthorRepository.FindByIdAsync(userId);

                if (author == null)
                {
                    _logger.LogCritical(_eventId, "COMMENT REPLY FAILED", $"Failed to find author profile for user to reply to comment {model.ReplyCommentId}.");
                    return Redirection(returnUrl);
                }

                Comment? commentToReply = await _db.CommentRepository.FindByIdAsync(model.ReplyCommentId);

                if (commentToReply == null)
                {
                    _logger.LogCritical(_eventId, "COMMENT REPLY FAILED", $"Failed to find comment.");
                    return Redirection(returnUrl);
                }

                Comment comment = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    AuthorId = author.Id,
                    Author = author,
                    BodyContent = model.BodyContent,
                    ParentCommentId = commentToReply.ParentCommentId,
                    ParentComment = commentToReply.ParentComment,
                };

                if (model.BodyContent.Length < 5 && model.BodyContent.Length > 500)
                {
                    _logger.LogWarning(_eventId, "COMMENT REPLY FAILED", $"Comment is too short or long to reply to comment.");
                    return Redirection(returnUrl);
                }

                //if (!TryValidateModel(comment))
                //{
                //    _logger.LogWarning(_eventId, $"COMMENT REPLY FAILED -> Comment is too long to reply to comment {model.ReplyCommentId}.");
                //    return Redirection(returnUrl);
                //}

                commentToReply.Replies.Add(comment);
                //int resultAdd = await _db.CommentRepository.AddAsync(comment);
                int resultUpdate = await _db.CommentRepository.UpdateAsync(commentToReply);
                if (resultUpdate <= 0)
                    _logger.LogCritical(_eventId, "COMMENT REPLY FAILED", $"Database failed to add reply to comment.");
                else
                    _logger.LogInformation(_eventId, "COMMENT REPLY SUCCESS", $"Successfully replied to comment.");
            }

            return Redirection(returnUrl);
        }

        private IActionResult Redirection(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}