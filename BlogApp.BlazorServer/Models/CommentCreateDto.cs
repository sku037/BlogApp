namespace BlogApp.BlazorServer.Models
{
    public class CommentCreateDto
    {
        public string Text { get; set; }
        public int PostId { get; set; }
        public string Username { get; set; }
    }

}
