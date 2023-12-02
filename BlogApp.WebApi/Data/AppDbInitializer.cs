using BlogApp.WebApi.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.WebApi.Data
{
    public class AppDbInitializer
    {
        public static async Task Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Seed User
                var user = await userManager.FindByEmailAsync("bruker1@uit.no");
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = "bruker1@uit.no",
                        FirstName = "Morten",
                        LastName = "Solberg",
                        Email = "bruker1@uit.no",
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, "Dayskipper#1");
                }

                // Seed Blogs, Posts, Comments, Tags
                if (!context.Blogs.Any())
                {
                    var blog = new Blog
                    {
                        Title = "Blog 1",
                        Description = "Description for Blog 1",
                        UserId = user.Id
                    };

                    context.Blogs.Add(blog);
                    await context.SaveChangesAsync(); // Save blog to get BlogId

                    var post = new Post
                    {
                        Content = "Content for Post 1 in Blog 1",
                        BlogId = blog.BlogId
                    };

                    context.Posts.Add(post);
                    await context.SaveChangesAsync(); // Save post to get PostId

                    var comment = new Comment
                    {
                        Text = "Comment for Post 1",
                        PostId = post.PostId
                    };

                    var tag = new Tag
                    {
                        Name = "#Technology"
                    };

                    context.Comments.Add(comment);
                    context.Tags.Add(tag);

                    await context.SaveChangesAsync(); // Save changes for comments and tags
                }
            }
        }
    }
}
