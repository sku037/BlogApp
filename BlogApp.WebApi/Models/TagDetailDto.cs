namespace BlogApp.WebApi.Models
{
    public class TagDetailDto
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public List<PostDto> Posts { get; set; } // Post list with tags
    }



}
