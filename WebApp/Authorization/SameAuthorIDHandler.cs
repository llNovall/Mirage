using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Authorization
{
    public class SameAuthorIDHandler : AuthorizationHandler<SameAuthorRequirement, Guid>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public SameAuthorIDHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameAuthorRequirement requirement, Guid authorId)
        {
            string? userId = _userManager.GetUserId(context.User);

            if (!string.IsNullOrEmpty(userId))
            {
                if (Guid.Parse(userId) == authorId)
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}