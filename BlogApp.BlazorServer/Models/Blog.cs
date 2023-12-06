using Microsoft.Extensions.Hosting;

namespace BlogApp.BlazorServer.Models
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        // Foreign Key for User
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Relationship with Posts
        public virtual ICollection<Post> Posts { get; set; }
    }
}
