using BlogApp.BlazorServer.Services;
using Microsoft.Extensions.Hosting;

namespace BlogApp.BlazorServer.Models
{
    public class BlogDto: IResourceOwner
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Username { get; set; }
        public string OwnerUsername => Username;
    }

}
