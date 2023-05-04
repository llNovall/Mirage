using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Authorization
{
    public class SameAuthorCommentHandler : AuthorizationHandler<SameAuthorRequirement, Comment>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public SameAuthorCommentHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAuthorRequirement requirement, Comment resource)
        {
            string? userId = _userManager.GetUserId(context.User);

            if (resource != null)
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    if (userId == resource.AuthorId)
                        context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}