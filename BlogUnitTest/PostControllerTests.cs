using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogApp.WebApi.Models;
using BlogApp.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace BlogUnitTest
{
    [TestClass]
    public class PostControllerTests
    {
        private PostController _controller;
        private ApplicationDbContext _context;
        private Mock<IWebHostEnvironment> _mockEnvironment;

        [TestInitialize]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Adding users
            var user1 = new ApplicationUser { Id = "user1", UserName = "User1", FirstName = "First1", LastName = "Last1" };
            var user2 = new ApplicationUser { Id = "user2", UserName = "User2", FirstName = "First2", LastName = "Last2" };
            _context.Users.AddRange(user1, user2);

            // Adding blogs
            var blog1 = new Blog { BlogId = 1, Title = "Blog 1", Description = "Description for Blog 1", UserId = "user1" };
            var blog2 = new Blog { BlogId = 2, Title = "Blog 2", Description = "Description for Blog 2", UserId = "user2" };
            _context.Blogs.AddRange(blog1, blog2);

            // Adding posts with ImagePath
            var post1 = new Post { PostId = 1, PostTitle = "Post 1", Content = "Content for Post 1", BlogId = 1, PublishDate = DateTime.UtcNow, ImagePath = "default-image1.jpg" };
            var post2 = new Post { PostId = 2, PostTitle = "Post 2", Content = "Content for Post 2", BlogId = 2, PublishDate = DateTime.UtcNow, ImagePath = "default-image2.jpg" };
            _context.Posts.AddRange(post1, post2);

            _context.SaveChanges();

            // Mock for IWebHostEnvironment
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockEnvironment.Setup(m => m.WebRootPath).Returns("path/to/webroot"); // Set the path to the webroot directory

            // Initialize PostController
            _controller = new PostController(_context, _mockEnvironment.Object);
        }


        //[TestMethod]
        //public async Task GetPosts_ShouldReturnAllPosts()
        //{
        //    // Act
        //    var result = await _controller.GetPosts();

        //    // Assert
        //    Assert.IsNotNull(result, "Response from GetPosts is null.");
        //    var posts = result.Value;
        //    Assert.IsNotNull(posts, "Posts list is null.");
        //    Assert.AreEqual(2, posts.Count(), "Number of posts does not match.");
        //}

        [TestMethod]
        public async Task GetPost_ShouldReturnPost_WhenExists()
        {
            // Arrange
            int postId = 1; // Ensure that a post with this ID exists in the database

            // Act
            var actionResult = await _controller.GetPost(postId);

            // Assert
            Assert.IsNotNull(actionResult, "Response from GetPost is null.");

            if (actionResult.Result is NotFoundResult)
            {
                Assert.Fail("Expected PostDetailDto, but got NotFoundResult.");
            }

            var postResult = actionResult.Value;
            Assert.IsNotNull(postResult, "PostDetailDto is null.");
            Assert.AreEqual(postId, postResult.PostId, "Post ID does not match.");
        }

        [TestMethod]
        public async Task GetPost_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            int postId = 100; // An ID that definitely does not exist

            // Act
            var actionResult = await _controller.GetPost(postId);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostPost_ShouldCreateNewPost()
        {
            // Arrange
            var newPost = new PostCreateDto
            {
                PostTitle = "New Post",
                Content = "Content for new post",
                BlogId = 1, // Assuming a blog with this ID exists
                TagNames = new List<string> { "Tag1", "Tag2" }
            };

            // Act
            var actionResult = await _controller.PostPost(newPost);

            // Assert
            Assert.IsNotNull(actionResult);
            var createdAtActionResult = actionResult.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            var post = createdAtActionResult.Value as PostDetailDto;
            Assert.IsNotNull(post);
            Assert.AreEqual(newPost.PostTitle, post.PostTitle);
        }

        [TestMethod]
        public async Task PutPost_ShouldUpdatePost_WhenExists()
        {
            // Arrange
            var existingPostId = 1; // Assuming a post with this ID exists
            var updatedPost = new PostEditDto
            {
                PostTitle = "Updated Title",
                Content = "Updated Content"
            };

            // Act
            var result = await _controller.PutPost(existingPostId, updatedPost);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task PutPost_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var nonExistingPostId = 100;
            var updatedPost = new PostEditDto
            {
                PostTitle = "Updated Title",
                Content = "Updated Content"
            };

            // Act
            var result = await _controller.PutPost(nonExistingPostId, updatedPost);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeletePost_ShouldRemovePost_WhenExists()
        {
            // Arrange
            var existingPostId = 1; // Assuming a post with this ID exists

            // Act
            var result = await _controller.DeletePost(existingPostId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeletePost_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var nonExistingPostId = 100;

            // Act
            var result = await _controller.DeletePost(nonExistingPostId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
