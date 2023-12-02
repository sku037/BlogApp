using BlogApp.WebApi.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.WebApi.Data
{
    public class AppDbInitializerUser
    {
        public static async Task Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Seed User
                if (!context.Users.Any())
                {
                    var user = new ApplicationUser
                    {
                        UserName = "bruker1@uit.no",
                        FirstName = "Morten",
                        LastName = "Solberg",
                        Email = "bruker1@uit.no",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, "Dayskipper#1");

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException("Error while creating seed user: " +
                                                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }
    }
}
