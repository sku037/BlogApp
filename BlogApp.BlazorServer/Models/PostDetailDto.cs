using BlogApp.BlazorServer.Services;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;

namespace BlogApp.BlazorServer.Models
{
    public class PostDetailDto: IResourceOwner
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }
        public string Username { get; set; }
        public int BlogId { get; set; }
        public List<string> TagNames { get; set; }
        public List<TagDto> Tags { get; set; }
        public string ImagePath { get; set; }
        public string OwnerUsername => Username;
    }

}
