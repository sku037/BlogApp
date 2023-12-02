using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.EntityFrameworkCore;
using BlogApp.WebApi.Controllers;
using BlogApp.WebApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BlogUnitTest
{
    [TestClass]
    public class BlogControllerTests
    {
        [TestMethod]
        public async Task GetBlogs_ReturnsAllBlogs()
        {
            // Arrange
            var testData = GetTestBlogs().AsQueryable();

            var mockSet = new Mock<DbSet<Blog>>();
            mockSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(testData.Provider);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(testData.Expression);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(testData.ElementType);
            mockSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Blogs).Returns(mockSet.Object); // Ensure your Blogs property is virtual

            var controller = new BlogController(mockContext.Object);

            // Act
            var result = await controller.GetBlogs();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Result is not OkObjectResult");

            var blogs = okResult.Value as IEnumerable<Blog>;
            Assert.IsNotNull(blogs, "Result Value is not IEnumerable<Blog>");

            Assert.AreEqual(3, blogs.Count(), "Expected number of blogs does not match");

        }


        private IQueryable<Blog> GetTestBlogs()
        {
            var blogs = new List<Blog>
            {
                new Blog { BlogId = 1, Title = "Test Blog 1", Description = "A test blog" },
                new Blog { BlogId = 2, Title = "Test Blog 2", Description = "Another test blog" },
                new Blog { BlogId = 3, Title = "Test Blog 3", Description = "Yet another test blog" }
            };

            return blogs.AsQueryable();
        }
    }
}
