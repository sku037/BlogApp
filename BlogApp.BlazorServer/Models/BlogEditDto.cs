using BlogApp.BlazorServer.Services;
using Microsoft.Extensions.Hosting;

namespace BlogApp.BlazorServer.Models
{
    public class BlogEditDto: IResourceOwner
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public string OwnerUsername => Username;
    }
}
