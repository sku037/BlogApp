namespace BlogApp.WebApi.Models
{
    public class SearchResultDetailDto
    {
        public string Id { get; set; } // Or use int if your identifier is numeric
        public string Title { get; set; } // The title of the post, user, or tag
        public string Content { get; set; } // The full text of the post or a description
        // Assuming it's a post, it might contain associated comments or tags
        public IEnumerable<string> Tags { get; set; } // Tags associated with the post
        public IEnumerable<string> Comments { get; set; } // Comments on the post
        // Any other details that may be important for your application
    }
}
