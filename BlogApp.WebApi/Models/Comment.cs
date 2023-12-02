namespace BlogApp.WebApi.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        // Foreign Key for Post
        public int PostId { get; set; }
        public virtual Post Post { get; set; }

        // Foreign Key for User
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

}
