using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using BlogApp.BlazorServer.Models;

namespace BlogApp.BlazorServer.Models
{
    public class PostDto
    {
        public int PostId { get; set; }
        public int BlogId { get; set; }
        public string PostTitle { get; set; }
        public DateTime PublishDate { get; set; }
        public List<TagDto> Tags { get; set; }
    }

}
