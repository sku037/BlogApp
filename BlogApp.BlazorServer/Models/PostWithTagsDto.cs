using Microsoft.Extensions.Hosting;
using System.Xml.Linq;

namespace BlogApp.BlazorServer.Models
{
    public class PostWithTagsDto
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public DateTime PublishDate { get; set; }
        public List<TagDto> Tags { get; set; } 
    }

}
