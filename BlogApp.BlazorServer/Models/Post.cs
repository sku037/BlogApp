using Microsoft.Extensions.Hosting;
using System.Xml.Linq;

namespace BlogApp.BlazorServer.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }

        // Foreign Key for Blog
        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        // Relationship with Comments
        public virtual ICollection<Comment> Comments { get; set; }

        // Relationship with Tags
        public virtual ICollection<Tag> Tags { get; set; }
    }

}
