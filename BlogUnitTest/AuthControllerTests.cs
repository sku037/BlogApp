using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BlogApp.WebApi.Models;
using BlogApp.WebApi.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.AspNetCore.Http;

namespace BlogUnitTest
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IConfiguration> _mockConfiguration;
        private AuthController _controller;

        [TestInitialize]
        public void SetUp()
        {
            // Mocking UserManager
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            // Mocking IConfiguration
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.SetupGet(c => c["JwtSettings:SecretKey"]).Returns("YourSecretKeyHere");

            // Initialize AuthController with mocked UserManager and IConfiguration
            _controller = new AuthController(_mockUserManager.Object, _mockConfiguration.Object);
        }

        [TestMethod]
        public async Task Login_ShouldReturnToken_WhenValidUser()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "testuser", Password = "password" };
            var user = new ApplicationUser { UserName = "testuser" };

            _mockUserManager.Setup(x => x.FindByNameAsync(loginModel.Username)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginModel.Password)).ReturnsAsync(true);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
        }

        [TestMethod]
        public async Task Login_ShouldReturnUnauthorized_WhenInvalidUser()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "invaliduser", Password = "password" };

            _mockUserManager.Setup(x => x.FindByNameAsync(loginModel.Username)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task Register_ShouldReturnSuccess_WhenNewUser()
        {
            // Arrange
            var registerModel = new RegisterModel { Username = "newuser", Email = "newuser@test.com", Password = "password", FirstName = "First", LastName = "Last" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(registerModel.Email)).ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerModel.Password)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Register_ShouldReturnError_WhenUserExists()
        {
            // Arrange
            var registerModel = new RegisterModel { Username = "existinguser", Email = "existinguser@test.com", Password = "password", FirstName = "First", LastName = "Last" };
            var existingUser = new ApplicationUser();

            _mockUserManager.Setup(x => x.FindByEmailAsync(registerModel.Email)).ReturnsAsync(existingUser);

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        // Additional test methods for GetUserById can be implemented here
    }
}
