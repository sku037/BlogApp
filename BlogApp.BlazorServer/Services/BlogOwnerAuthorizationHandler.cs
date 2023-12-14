using BlogApp.BlazorServer.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlogApp.BlazorServer.Services
{
    public class ResourceOwnerAuthorizationHandler :
        AuthorizationHandler<ResourceOwnerRequirement, IResourceOwner>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ResourceOwnerRequirement requirement,
            IResourceOwner resource)
        {
            if (context.User.Identity?.Name == resource.OwnerUsername)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
