using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogApp.WebApi.Models;
using BlogApp.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BlogUnitTest
{
    [TestClass]
    public class TagControllerTests
    {
        private TagController _controller;
        private ApplicationDbContext _context;

        [TestInitialize]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Creating a unique In-Memory Database for each test
                .Options;

            _context = new ApplicationDbContext(options);

            // Ensure the database is empty before adding new items
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Adding unique tags
            _context.Tags.AddRange(
                new Tag { TagId = 1, Name = "Technology" },
                new Tag { TagId = 2, Name = "Programming" }
            );

            _context.SaveChanges();

            _controller = new TagController(_context);
        }

        [TestMethod]
        public async Task GetTags_ShouldReturnAllTags()
        {
            // Act
            var result = await _controller.GetTags();

            // Assert
            Assert.IsNotNull(result.Value);
            var tags = result.Value.ToList();
            Assert.AreEqual(2, tags.Count); // Expecting 2 tags
        }

        [TestMethod]
        public async Task GetTag_ShouldReturnTag_WhenExists()
        {
            // Arrange
            int tagId = 1;

            // Act
            var result = await _controller.GetTag(tagId);

            // Assert
            Assert.IsNotNull(result.Value);
            var tag = result.Value;
            Assert.AreEqual(tagId, tag.TagId);
            Assert.AreEqual("Technology", tag.Name);
        }

        [TestMethod]
        public async Task GetTag_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            int tagId = 100;

            // Act
            var result = await _controller.GetTag(tagId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostTag_ShouldCreateNewTag()
        {
            // Arrange
            var newTag = new TagCreateDto { Name = "Science" };

            // Act
            var result = await _controller.PostTag(newTag);

            // Assert
            Assert.IsNotNull(result);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            var tag = createdAtActionResult.Value as TagDto;
            Assert.IsNotNull(tag);
            Assert.AreEqual(newTag.Name, tag.Name);
        }

        [TestMethod]
        public async Task PutTag_ShouldUpdateTag_WhenExists()
        {
            // Arrange
            var existingTagId = 1; // Ensure that tag with this ID exists
            var existingTag = _context.Tags.Find(existingTagId); // Retrieving existing tag from the context
            existingTag.Name = "Updated Name"; // Updating tag name

            // Act
            var result = await _controller.PutTag(existingTagId, existingTag);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            // Verifying that data was updated
            var updatedTag = _context.Tags.Find(existingTagId);
            Assert.AreEqual("Updated Name", updatedTag.Name);
        }

        [TestMethod]
        public async Task PutTag_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var tagId = 100;
            var tagToUpdate = new Tag { TagId = tagId, Name = "Nonexistent Tag" };

            // Act
            var result = await _controller.PutTag(tagId, tagToUpdate);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteTag_ShouldRemoveTag_WhenExists()
        {
            // Arrange
            var tagId = 1;

            // Act
            var result = await _controller.DeleteTag(tagId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.IsNull(_context.Tags.Find(tagId));
        }

        [TestMethod]
        public async Task DeleteTag_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var tagId = 100;

            // Act
            var result = await _controller.DeleteTag(tagId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
