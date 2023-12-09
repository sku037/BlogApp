using Microsoft.Extensions.Hosting;

namespace BlogApp.WebApi.Models
{
    public class BlogDto
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
