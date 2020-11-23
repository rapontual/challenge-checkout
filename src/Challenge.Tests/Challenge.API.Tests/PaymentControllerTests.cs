using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Challenge.API.Controllers;
using Challenge.Core.DTO;
using Challenge.Data.Repository.Interfaces;
using Challenge.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Challenge.Tests.Challenge.API.Tests
{
    public class PaymentControllerTests
    {
        private static Guid merchantId = Guid.NewGuid();

        private Mock<IPaymentTransactionRepository> mockRepository;
        private Mock<IApprovePaymentService> mockPaymentService;
        private PaymentController controller;

        [SetUp]
        public void Setup()
        {
            this.mockRepository = new Mock<IPaymentTransactionRepository>();
            
            this.mockPaymentService = new Mock<IApprovePaymentService>();

            this.mockPaymentService
                .Setup(s => s.ValidateCard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(string.Empty);

            this.controller = new PaymentController(this.mockRepository.Object, this.mockPaymentService.Object);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "login"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.UserData, merchantId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            this.controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public void PaymentController_InvalidThoken_ShouldReturnBadRequest()
        {
            // Arrange
            this.SetNotAuthenticated();

            // Act 
            var response = this.controller.Post(new PaymentCreateRequestDTO());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public void PaymentController_InvalidPayment_ShouldReturnBadRequest()
        {
            // Arrange
            this.mockPaymentService
                .Setup(s => s.ValidateCard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("invalid");

            // Act 
            var response = this.controller.Post(new PaymentCreateRequestDTO());

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public void PaymentController_Payment_ShouldReturnResponseOk()
        {
            // Arrange
            var paymentTransacation = new Core.Model.PaymentTransaction
            {
                Id = Guid.NewGuid(),
                Status = new Core.Model.PaymentStatus
                {
                    StatusDescription = "Approved"
                }
            };

            this.mockPaymentService
                .Setup(s => s.ApprovePayment(It.IsAny<Core.Model.PaymentTransaction>()))
                .Returns(paymentTransacation);

            // Act 
            var response = this.controller.Post(new PaymentCreateRequestDTO());

            // Assert
            Assert.IsInstanceOf<JsonResult>(response);

            var responseValue = ((Microsoft.AspNetCore.Mvc.JsonResult)response).Value;
            Assert.NotNull(responseValue);

            var paymentResponse = responseValue as PaymentCreateResponseDTO;
            Assert.NotNull(paymentResponse);
            Assert.AreEqual(paymentTransacation.Id, paymentResponse.TransactionId);
        }

        private void SetNotAuthenticated()
        {
            this.controller.ControllerContext = new ControllerContext();
        }
    }
}
