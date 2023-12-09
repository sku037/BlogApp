namespace BlogApp.BlazorServer.Models
{
    public class CommentDetailDto
    {
        public int CommentId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string Username { get; set; } 
    }

}
