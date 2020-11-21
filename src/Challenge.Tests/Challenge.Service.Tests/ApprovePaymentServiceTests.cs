using System;
using Challenge.Core.Model;
using Challenge.Data.Repository.Interfaces;
using Challenge.Service;
using Challenge.Service.BankClient;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Challenge.Tests.Service
{
    public class ApprovePaymentServiceTests
    {
        private Mock<IBankApprovalService> mockBankApprovalService;
        private Mock<IPaymentTransactionRepository> mockRepository;
        private Mock<ILogger<ApprovePaymentService>> mockLogger;
        private ApprovePaymentService target;
        private Guid bankTransacationId = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            this.mockBankApprovalService = new Mock<IBankApprovalService>();
            this.mockRepository = new Mock<IPaymentTransactionRepository>();
            this.mockLogger = new Mock<ILogger<ApprovePaymentService>>();

            this.mockBankApprovalService
                .Setup(s => s.Approve(It.IsAny<BankApprovalRequest>()))
                .Returns(new BankApprovalResponse
                {
                    Status = BankAporovalStatus.Approved,
                    TransactionId = bankTransacationId
                });

            target = new ApprovePaymentService(
                this.mockBankApprovalService.Object, 
                this.mockRepository.Object, 
                this.mockLogger.Object);
        }

        [Test]
        public void ApprovePaymentService_BankServiceApprovePayment_ShouldHitDependenciesAndWrite()
        {
            // Arrange
            var input = new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                Amout = 12567.88,
                CardNumber = "1111222233334444",
                CVV = "2233",
                Merchant = new Merchant
                {
                    Id = Guid.NewGuid()
                }
            };

            // Act
            var result = target.ApprovePayment(input);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Status.Id);

            this.mockBankApprovalService
                .Verify(v => v.Approve(It.Is<BankApprovalRequest>(b => b.Amout.Equals(input.Amout))), Times.Once);

            this.mockRepository
                .Verify(v => v.Add(It.Is<PaymentTransaction>(p => p.BankTransactionId.Equals(bankTransacationId))), Times.Once);
        }

        [Test]
        public void ApprovePaymentService_BankServiceThorwsException_ShouldHitDependenciesAndWriteWithErroStatus()
        {
            // Arrange
            var input = new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                Amout = 12567.88,
                CardNumber = "1111222233334444",
                CVV = "2233",
                Merchant = new Merchant
                {
                    Id = Guid.NewGuid()
                }
            };

            this.mockBankApprovalService
                .Setup(s => s.Approve(It.IsAny<BankApprovalRequest>()))
                .Throws(new Exception());

            // Act
            var result = target.ApprovePayment(input);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(4, result.Status.Id);

            this.mockBankApprovalService
                .Verify(v => v.Approve(It.Is<BankApprovalRequest>(b => b.Amout.Equals(input.Amout))), Times.Once);

            this.mockRepository
                .Verify(v => v.Add(It.Is<PaymentTransaction>(p => p.BankTransactionId.Equals(default(Guid)))), Times.Once);
        }

        [TestCase("1111222233334444","1222","123","Owner", true)]
        [TestCase("", "1222", "123", "Owner", false)]
        [TestCase("1111222233334444", "", "123", "Owner", false)]
        [TestCase("1111222233334444", "1222", "1", "Owner", false)]
        [TestCase("1111222233334444", "1222", "123", "", false)]
        public void ApprovePaymentService_ValidateCard_ShouldValidateCorrectly(string cardNumber, string expire, string cvv, string owernName, bool expectedEmpty)
        {
            // Act
            var result = target.ValidateCard(cardNumber, expire, cvv, owernName);

            // Assert
            Assert.AreEqual(expectedEmpty, string.IsNullOrWhiteSpace(result));

        }
    }
}