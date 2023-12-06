using Microsoft.AspNetCore.Identity;

namespace BlogApp.BlazorServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        

        // relation with blogs
        public virtual ICollection<Blog> Blogs { get; set; }

        // relation with blogs
        public virtual ICollection<Comment> Comments { get; set; }

    }
}
