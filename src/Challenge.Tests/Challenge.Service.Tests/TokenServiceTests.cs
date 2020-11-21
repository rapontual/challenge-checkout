using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Challenge.Core.Model;
using Challenge.Core.Settings;
using Challenge.Service;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Challenge.Tests.Challenge.Service.Tests
{
    public class TokenServiceTests
    {
        private TokenService service;
        private Mock<IOptions<ChallengeSettings>> mockSettings;

        [SetUp]
        public void Setup()
        {
            this.mockSettings = new Mock<IOptions<ChallengeSettings>>();
            this.mockSettings
                .SetupGet(s => s.Value)
                .Returns(new ChallengeSettings
                {
                    SecretKey = "4368406c6c656e676539536563726574"
                });

            this.service = new TokenService(mockSettings.Object);
        }

        [Test]
        public void TokenService_PassMerchantAdmin_ShouldReturnToken()
        {
            // Arrange
            var merchant = new Merchant
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                Login = "test"
            };

            // Act
            var token = this.service.GenerateToken(merchant);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));

            var tokenObject = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var merchantId = tokenObject.Claims.Where(a => a.Type == ClaimTypes.UserData)
               .FirstOrDefault()?.Value;

            Assert.AreEqual(merchant.Id.ToString(), merchantId);

        }
    }
}
