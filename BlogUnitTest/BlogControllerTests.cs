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
            // Generate a unique database name for each test run
            var dbName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new ApplicationDbContext(options);

            // Add test data to the in-memory database
            context.Blogs.AddRange(
                new Blog
                {
                    BlogId = 1,
                    Title = "Test Blog 1",
                    Description = "Description of Test Blog 1",
                    CreatedDate = new DateTime(2022, 1, 1),
                    UserId = "user1"
                },
                new Blog
                {
                    BlogId = 2,
                    Title = "Test Blog 2",
                    Description = "Description of Test Blog 2",
                    CreatedDate = new DateTime(2022, 1, 2),
                    UserId = "user2"
                });

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
            var model = result.Value; // Изменение здесь
            Assert.IsNotNull(model);
            var blogList = model.ToList();
            Assert.AreEqual(2, blogList.Count); // Проверяем, что возвращаются два блога
        }

        [TestMethod]
        public async Task GetBlogs_ShouldReturnEmptyListWhenNoBlogs()
        {
            // Arrange - Создаем новую пустую In-Memory Database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя для новой базы данных
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
            Assert.AreEqual(0, blogList.Count); // Проверяем, что список блогов пуст
        }

        [TestMethod]
        public async Task GetBlog_ShouldReturnBlog_WhenExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            var existingBlog = new Blog
            {
                BlogId = 1,
                Title = "Test Blog 1",
                Description = "Description of Test Blog 1",
                CreatedDate = new DateTime(2022, 1, 1),
                UserId = "user1"
            };
            context.Blogs.Add(existingBlog);
            context.SaveChanges();

            var controller = new BlogController(context);

            // Act
            var result = await controller.GetBlog(existingBlog.BlogId);

            // Assert
            Assert.IsNotNull(result);
            var blog = result.Value; // Напрямую обращаемся к Value
            Assert.IsNotNull(blog);
            Assert.AreEqual(existingBlog.BlogId, blog.BlogId);
        }

        [TestMethod]
        public async Task GetBlog_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var blogId = 100; // ID несуществующего блога

            // Act
            var result = await _controller.GetBlog(blogId);

            // Assert
            Assert.IsNotNull(result);
            var notFoundResult = result.Result as NotFoundResult; // Используйте result.Result для доступа к ActionResult
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
