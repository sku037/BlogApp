using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogApp.WebApi.Models;
using BlogApp.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BlogUnitTest
{
    [TestClass]
    public class CommentControllerTests
    {
        private CommentController _controller;
        private ApplicationDbContext _context;

        [TestInitialize]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Adding users
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "user1", UserName = "User1", FirstName = "First1", LastName = "Last1" },
                new ApplicationUser { Id = "user2", UserName = "User2", FirstName = "First2", LastName = "Last2" }
            };
            _context.Users.AddRange(users);

            // Adding blogs
            var blogs = new List<Blog>
            {
                new Blog { BlogId = 1, Title = "Blog 1", Description = "Description for Blog 1", UserId = "user1" },
                new Blog { BlogId = 2, Title = "Blog 2", Description = "Description for Blog 2", UserId = "user2" }
            };
            _context.Blogs.AddRange(blogs);

            // Adding posts
            var posts = new List<Post>
            {
                new Post { PostId = 1, PostTitle = "Post 1", Content = "Content for Post 1", BlogId = 1, PublishDate = DateTime.UtcNow },
                new Post { PostId = 2, PostTitle = "Post 2", Content = "Content for Post 2", BlogId = 2, PublishDate = DateTime.UtcNow }
            };
            _context.Posts.AddRange(posts);

            // Adding comments
            var comments = new List<Comment>
            {
                new Comment { CommentId = 1, Text = "Comment 1", Date = DateTime.UtcNow, PostId = 1, UserId = "user1" },
                new Comment { CommentId = 2, Text = "Comment 2", Date = DateTime.UtcNow, PostId = 2, UserId = "user2" }
            };
            _context.Comments.AddRange(comments);

            _context.SaveChanges();

            _controller = new CommentController(_context);
        }

        [TestMethod]
        public async Task GetComments_ShouldReturnAllComments()
        {
            // Act
            var result = await _controller.GetComments();

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.IsTrue(result.Value.Count() > 0);
        }

        [TestMethod]
        public async Task GetComment_ShouldReturnComment_WhenExists()
        {
            // Arrange
            int commentId = 1; // Ensure that a comment with this ID exists

            // Act
            var result = await _controller.GetComment(commentId);

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(commentId, result.Value.CommentId);
        }

        [TestMethod]
        public async Task GetComment_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            int commentId = 100; // Non-existing ID

            // Act
            var result = await _controller.GetComment(commentId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostComment_ShouldCreateNewComment()
        {
            // Arrange
            var newComment = new CommentController.CommentCreateDto
            {
                Text = "New comment",
                PostId = 1, // Ensure this post exists in the test database
                Username = "User1" // Use the username created in SetUp
            };

            // Act
            var actionResult = await _controller.PostComment(newComment);

            // Assert
            Assert.IsNotNull(actionResult, "Response from PostComment is null.");

            if (actionResult.Result is CreatedAtActionResult createdAtActionResult)
            {
                var comment = createdAtActionResult.Value as CommentDetailDto;
                Assert.IsNotNull(comment, "Created comment is null.");
                Assert.AreEqual(newComment.Text, comment.Text, "Comment text does not match.");
            }
            else if (actionResult.Result is NotFoundResult)
            {
                Assert.Fail("Expected CreatedAtActionResult, but got NotFoundResult. Check if Post/User exist.");
            }
            else
            {
                Assert.Fail("Expected CreatedAtActionResult, but got a different type.");
            }
        }


        [TestMethod]
        public async Task PutComment_ShouldUpdateComment_WhenExists()
        {
            // Arrange
            var existingComment = _context.Comments.First(c => c.CommentId == 1); // Getting the existing comment
            existingComment.Text = "Updated comment"; // Updating comment text

            // Act
            var result = await _controller.PutComment(existingComment.CommentId, existingComment);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            // Checking if the data has been updated
            var updatedComment = _context.Comments.Find(existingComment.CommentId);
            Assert.AreEqual("Updated comment", updatedComment.Text);
        }


        [TestMethod]
        public async Task PutComment_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var nonExistingCommentId = 100;
            var updatedComment = new Comment { CommentId = nonExistingCommentId, Text = "Updated comment" };

            // Act
            var result = await _controller.PutComment(nonExistingCommentId, updatedComment);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteComment_ShouldRemoveComment_WhenExists()
        {
            // Arrange
            var existingCommentId = 1; // Assuming the comment exists

            // Act
            var result = await _controller.DeleteComment(existingCommentId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteComment_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var nonExistingCommentId = 100;

            // Act
            var result = await _controller.DeleteComment(nonExistingCommentId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetCommentsByPostId_ShouldReturnCommentsForPost()
        {
            // Arrange
            int postId = 1; // Assuming the post with this ID exists

            // Act
            var result = await _controller.GetCommentsByPostId(postId);

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.IsTrue(result.Value.Any());
        }
    }
}
