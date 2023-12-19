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
    public class BlogControllerTests
    {
        private BlogController _controller;

        [TestInitialize]
        public void SetUp()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new ApplicationDbContext(options);

            // Add test user data
            var user1 = new ApplicationUser
            {
                Id = "user1",
                UserName = "User1",
                FirstName = "FirstName1", // Set required FirstName
                LastName = "LastName1"    // Set required LastName
                // Set any other required properties
            };
            var user2 = new ApplicationUser
            {
                Id = "user2",
                UserName = "User2",
                FirstName = "FirstName2", // Set required FirstName
                LastName = "LastName2"    // Set required LastName
                // Set any other required properties
            };
            context.Users.AddRange(user1, user2);

            // Add test blog data
            context.Blogs.AddRange(
                new Blog
                {
                    BlogId = 1,
                    Title = "Test Blog 1",
                    Description = "Description of Test Blog 1",
                    CreatedDate = new DateTime(2022, 1, 1),
                    User = user1
                },
                new Blog
                {
                    BlogId = 2,
                    Title = "Test Blog 2",
                    Description = "Description of Test Blog 2",
                    CreatedDate = new DateTime(2022, 1, 2),
                    User = user2
                }
            );

            context.SaveChanges();
            _controller = new BlogController(context);
        }



        // Tests for GetBlogs
        [TestMethod]
        public async Task GetBlogs_ShouldReturnAllBlogs()
        {
            // Act
            var result = await _controller.GetBlogs();

            // Assert
            Assert.IsNotNull(result);
            var model = result.Value; // Change here
            Assert.IsNotNull(model);
            var blogList = model.ToList();
            Assert.AreEqual(2, blogList.Count); // Check 2 blogs are returned
        }

        [TestMethod]
        public async Task GetBlogs_ShouldReturnEmptyListWhenNoBlogs()
        {
            // Arrange - Создаем новую пустую In-Memory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for ney database
                .Options;

            var emptyContext = new ApplicationDbContext(options);
            var controller = new BlogController(emptyContext);

            // Act
            var result = await controller.GetBlogs();

            // Assert
            Assert.IsNotNull(result);
            var model = result.Value;
            Assert.IsNotNull(model);
            var blogList = model.ToList();
            Assert.AreEqual(0, blogList.Count); // Check blog list is empty
        }

        [TestMethod]
        public async Task GetBlog_ShouldReturnBlog_WhenExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            // Add a User entity with all required properties
            var user = new ApplicationUser
            {
                Id = "user1",
                UserName = "TestUser",
                FirstName = "FirstName", // Add a value for FirstName
                LastName = "LastName"    // Add a value for LastName
                // Add other necessary properties of ApplicationUser if required
            };
            context.Users.Add(user);

            // Add a Blog entity
            var existingBlog = new Blog
            {
                BlogId = 1,
                Title = "Test Blog 1",
                Description = "Description of Test Blog 1",
                CreatedDate = new DateTime(2022, 1, 1),
                UserId = user.Id // Use the Id of the user you just added
            };
            context.Blogs.Add(existingBlog);
            context.SaveChanges();

            var controller = new BlogController(context);

            // Act
            var result = await controller.GetBlog(existingBlog.BlogId);

            // Assert
            Assert.IsNotNull(result);
            var blog = result.Value; // Directly access the Value
            Assert.IsNotNull(blog);
            Assert.AreEqual(existingBlog.BlogId, blog.BlogId);
            Assert.AreEqual("TestUser", blog.Username); // Assert that the Username is correctly mapped
        }



        [TestMethod]
        public async Task GetBlog_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var blogId = 100; // blog ID which doesn't exist

            // Act
            var result = await _controller.GetBlog(blogId);

            // Assert
            Assert.IsNotNull(result);
            var notFoundResult = result.Result as NotFoundResult; // Use result.Result for access to ActionResult
            Assert.IsNotNull(notFoundResult, "Expected NotFoundResult, but got a different type.");
        }

        [TestMethod]
        public async Task PostBlog_ShouldCreateNewBlog_WhenUserExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            // Добавляем пользователя с необходимыми полями
            var existingUser = new ApplicationUser
            {
                Id = "user1",
                UserName = "existingUser",
                FirstName = "Test",  // Добавлено FirstName
                LastName = "User"    // Добавлено LastName
            };
            context.Users.Add(existingUser);
            context.SaveChanges();

            var controller = new BlogController(context);

            var newBlog = new BlogController.BlogCreateDto
            {
                Title = "New Blog",
                Description = "New Blog Description",
                Username = "existingUser"
            };

            // Act
            var result = await controller.PostBlog(newBlog);

            // Assert
            Assert.IsNotNull(result);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult, "Expected CreatedAtActionResult, but got a different type.");
            var blog = createdAtActionResult.Value as BlogDto;
            Assert.IsNotNull(blog, "Expected BlogDto, but got null.");
            Assert.AreEqual(newBlog.Title, blog.Title);
        }

        [TestMethod]
        public async Task PostBlog_ShouldReturnNotFound_WhenUserNotExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            var controller = new BlogController(context);

            var newBlog = new BlogController.BlogCreateDto
            {
                Title = "New Blog",
                Description = "New Blog Description",
                Username = "nonExistingUser"
            };

            // Act
            var result = await controller.PostBlog(newBlog);

            // Assert
            Assert.IsNotNull(result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult, but got a different type.");
        }

        [TestMethod]
        public async Task PutBlog_ShouldUpdateBlog_WhenExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            // Добавляем тестового пользователя с обязательными полями
            var user = new ApplicationUser
            {
                Id = "testUserId",
                UserName = "testUser",
                FirstName = "Test",  // Добавлено
                LastName = "User"    // Добавлено
                // Добавьте другие необходимые поля, если они есть
            };
            context.Users.Add(user);
            context.SaveChanges();

            var existingBlog = new Blog
            {
                BlogId = 1,
                Title = "Original Title",
                Description = "Original Description",
                UserId = user.Id // Указываем UserId
                // Добавьте другие необходимые поля блога, если они есть
            };
            context.Blogs.Add(existingBlog);
            context.SaveChanges();

            var controller = new BlogController(context);

            var updatedBlog = new BlogController.BlogEditDto
            {
                Title = "Updated Title",
                Description = "Updated Description"
            };

            // Act
            var result = await controller.PutBlog(existingBlog.BlogId, updatedBlog);

            // Assert
            Assert.IsNotNull(result);
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
        }



        [TestMethod]
        public async Task PutBlog_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            var controller = new BlogController(context);

            var nonExistingBlogId = 100;
            var updatedBlog = new BlogController.BlogEditDto
            {
                Title = "Updated Title",
                Description = "Updated Description"
            };

            // Act
            var result = await controller.PutBlog(nonExistingBlogId, updatedBlog);

            // Assert
            Assert.IsNotNull(result);
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }

        [TestMethod]
        public async Task DeleteBlog_ShouldRemoveBlog_WhenExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            var existingBlog = new Blog
            {
                BlogId = 1,
                Title = "Existing Blog",
                Description = "Existing Description",
                UserId = "user1" // Убедитесь, что это поле заполнено
                // Добавьте другие необходимые поля, если они есть
            };
            context.Blogs.Add(existingBlog);
            context.SaveChanges();

            var controller = new BlogController(context);

            // Act
            var result = await controller.DeleteBlog(existingBlog.BlogId);

            // Assert
            Assert.IsNotNull(result);
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
        }

        [TestMethod]
        public async Task DeleteBlog_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            var controller = new BlogController(context);

            var nonExistingBlogId = 100;

            // Act
            var result = await controller.DeleteBlog(nonExistingBlogId);

            // Assert
            Assert.IsNotNull(result);
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }


    }
}
