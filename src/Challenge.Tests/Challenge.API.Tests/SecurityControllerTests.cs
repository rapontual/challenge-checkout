using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Challenge.API.Controllers;
using Challenge.Core.Model;
using Challenge.Data.Repository.Interfaces;
using Challenge.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Challenge.Tests.Challenge.API.Tests
{
    public class SecurityControllerTests
    {
        private Mock<ISecurityRepository> mockRepository;
        private Mock<ITokenService> mockTokenService;
        private SecurityController controller;

        [SetUp]
        public void Setup()
        {
            this.mockRepository = new Mock<ISecurityRepository>();
            this.mockTokenService = new Mock<ITokenService>();
            
            this.controller = new SecurityController(this.mockRepository.Object, this.mockTokenService.Object);
        }

        [Test]
        public void SecurityController_Authenticate_InvalidCredentials_ShouldReturn404()
        {
            // Arrange
            this.mockRepository
                .Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(null as Merchant);

            // Act
            var response = this.controller.Authenticate(new Core.DTO.LoginDTO());

            var x = response.GetType();

            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public void SecurityController_Authenticate_ValidCredentials_ShouldReturn200()
        {
            // Arrange
            this.mockRepository
                .Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Merchant());

            // Act
            var response = this.controller
                .Authenticate(new Core.DTO.LoginDTO());

            Assert.IsInstanceOf<OkObjectResult>(response);
        }

        [Test]
        public void SecurityController_CreateLogin_InvalidUser_ShouldReturn404()
        {
            var response = this.controller.CreateLogin(new Core.DTO.UserDTO());

            var x = response.GetType();

            Assert.IsInstanceOf<BadRequestResult>(response);
        }

        [Test]
        public void SecurityController_CreateLogin_ValidUser_ShouldReturn200()
        {
            var userId = Guid.NewGuid();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "login"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.UserData, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            Thread.CurrentPrincipal = claimsPrincipal;

            var user = new Core.DTO.UserDTO
            {
                Login = "login",
                Password = "password"
            };

            var response = this.controller.CreateLogin(user);

            Assert.IsInstanceOf<JsonResult>(response);
        }
    }
}
