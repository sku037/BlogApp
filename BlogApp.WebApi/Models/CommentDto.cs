namespace BlogApp.WebApi.Models
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int PostId { get; set; }
    }


}
