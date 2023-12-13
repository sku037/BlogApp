using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.WebApi.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext // changed DbContext to get Users for AppDbInitializer
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationship conf between ApplicationUser and Blog
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Blogs) // ApplicationUser har mange Blogs
                .WithOne(b => b.User) // Blog har en User
                .HasForeignKey(b => b.UserId); // Foreign key in Blog - UserId

            // Relationship of Comment to Post
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post) // Comment har en Post
                .WithMany(p => p.Comments) // Post har mange Comments
                .HasForeignKey(c => c.PostId) // Foreign key in Comment - PostId
                .OnDelete(DeleteBehavior.Restrict); // Cascade delete change

            // Seeding is performed with AppDbInitializer

            //// Seeding data for Blogs
            //modelBuilder.Entity<Blog>().HasData(
            //    new Blog { BlogId = 1, Title = "Blog 1", Description = "Description for Blog 1" },
            //    new Blog { BlogId = 2, Title = "Blog 2", Description = "Description for Blog 2" }
            //);

            //// Seeding data for Posts
            //modelBuilder.Entity<Post>().HasData(
            //    new Post { PostId = 1, BlogId = 1, Content = "Content for Post 1 in Blog 1" },
            //    new Post { PostId = 2, BlogId = 1, Content = "Content for Post 2 in Blog 1" },
            //    new Post { PostId = 3, BlogId = 2, Content = "Content for Post 1 in Blog 2" }
            //);

            //// Seeding data for Comments
            //modelBuilder.Entity<Comment>().HasData(
            //    new Comment { CommentId = 1, PostId = 1, Text = "Comment for Post 1" },
            //    new Comment { CommentId = 2, PostId = 2, Text = "Comment for Post 2" }
            //);

            //// Seeding data for Tags
            //modelBuilder.Entity<Tag>().HasData(
            //    new Tag { TagId = 1, Name = "#Technology" },
            //    new Tag { TagId = 2, Name = "#Programming" }
            //);

            //// Seeding many-to-many relationship for Tags and Posts
            //modelBuilder.Entity<Post>()
            //    .HasMany(p => p.Tags)
            //    .WithMany(t => t.Posts)
            //    .UsingEntity(j => j.HasData(
            //        new { PostsPostId = 1, TagsTagId = 1 },
            //        new { PostsPostId = 1, TagsTagId = 2 },
            //        new { PostsPostId = 2, TagsTagId = 2 }
            //    ));

        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
