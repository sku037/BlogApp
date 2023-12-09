namespace BlogApp.WebApi.Models
{
    public class CommentDetailDto
    {
        public int CommentId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int PostId { get; set; }
        public string UserName { get; set; } 
    }
}
