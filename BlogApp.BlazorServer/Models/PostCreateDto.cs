using Microsoft.Extensions.Hosting;

namespace BlogApp.BlazorServer.Models
{
    public class PostCreateDto
    {
        public string PostTitle { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }
        
    }
}
