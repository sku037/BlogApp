using Microsoft.Extensions.Hosting;

namespace BlogApp.BlazorServer.Models
{
    public class PostEditDto
    {
        public string PostTitle { get; set; }
        public string Content { get; set; }
        
    }
}
