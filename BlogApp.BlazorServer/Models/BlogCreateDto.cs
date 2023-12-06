using Microsoft.Extensions.Hosting;

namespace BlogApp.BlazorServer.Models
{
    public class BlogCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
    }
}
