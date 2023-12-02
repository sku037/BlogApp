using BlogApp.WebApi.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.WebApi.Data
{
    public class AppDbInitializerRemoveUser
    {
        public static async Task Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var user = await userManager.FindByEmailAsync("bruker1@uit.no");

                // If user found delete it
                if (user != null)
                {
                    var result = await userManager.DeleteAsync(user);

                    if (!result.Succeeded)
                    {
                        // If errors under deletion
                        var errors = result.Errors.Select(e => e.Description).ToList();
                        throw new InvalidOperationException("The user can not be deleted: " + string.Join(", ", errors));
                    }
                }
            }
        }
    }
}
