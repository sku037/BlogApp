using Microsoft.Extensions.Hosting;

namespace BlogApp.WebApi.Models
{
    public class PostCreateDto
    {
        public string PostTitle { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }
        public List<int> TagIds { get; set; } = new List<int>();
        public List<string> TagNames { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }
}
