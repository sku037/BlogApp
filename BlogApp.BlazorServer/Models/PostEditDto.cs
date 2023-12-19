using BlogApp.BlazorServer.Services;
using Microsoft.Extensions.Hosting;

namespace BlogApp.BlazorServer.Models
{
    public class PostEditDto: IResourceOwner
    {
        public string PostTitle { get; set; }
        public string Content { get; set; }
        public List<int> TagIds { get; set; }
        public List<string> TagNames { get; set; }
        public string ImagePath { get; set; }
        public string Username { get; set; }
        public string OwnerUsername => Username;
    }
}
