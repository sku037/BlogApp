namespace BlogApp.BlazorServer.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }

        // Relationship with Posts
        public virtual ICollection<Post> Posts { get; set; }
    }

}
