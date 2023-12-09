using Microsoft.Extensions.Hosting;
using System.Xml.Linq;

namespace BlogApp.WebApi.Models
{
    public class PostDto
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public DateTime PublishDate { get; set; }
    }

}
