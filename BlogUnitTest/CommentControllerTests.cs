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
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new ApplicationDbContext(options);

            // Add test user data
            var user = new ApplicationUser
            {
                Id = "user1",
                UserName = "User1",
                FirstName = "First", // Set FirstName
                LastName = "Last" // Set LastName
            };
            context.Users.Add(user);

            // Add test blog data
            var blog = new Blog
            {
                BlogId = 1,
                Title = "Test Blog",
                Description = "Blog Description",
                UserId = user.Id, // Set UserId
                // Set any other required properties
            };
            context.Blogs.Add(blog);

            // Add test post data
            var post = new Post
            {
                PostId = 1,
                PostTitle = "Test Post",
                Content = "Content of Test Post",
                PublishDate = DateTime.UtcNow,
                ImagePath = "default-image-path.jpg", // Set ImagePath
                BlogId = blog.BlogId, // Link to the test blog
                
            };
            context.Posts.Add(post);

            // Add test comment data
            var comment = new Comment
            {
                CommentId = 1,
                Text = "Test Comment",
                Date = DateTime.UtcNow,
                PostId = post.PostId, // Associate with the test post
                UserId = user.Id, // Set UserId
                
            };
            context.Comments.Add(comment);

            context.SaveChanges();
            _controller = new CommentController(context);
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
