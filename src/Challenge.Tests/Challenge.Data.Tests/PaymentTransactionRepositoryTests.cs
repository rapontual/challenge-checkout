using System;
using System.Linq;
using System.Threading.Tasks;
using Challenge.Core.Model;
using Challenge.Data.Repository;
using NUnit.Framework;

namespace Challenge.Tests.Data
{
    public class PaymentTransactionRepositoryTests 
    {
        private PaymentTransactionRepository repository;

        [SetUp]
        public void Setup()
        {
            this.repository = new PaymentTransactionRepository(RepositoryTestsFixture.DbContext);
        }

        [Test]
        public void PaymentTransactionRepository_Add_ShouldAdd()
        {
            // Arrange
            var input = GetPaymentTransaction();

            // Act
            var id = this.repository.Add(input);

            // Assert
            Assert.AreNotEqual(default(Guid), id);
            var payment = this.repository.FindById(id); 
            Assert.IsNotNull(payment);
        }

        [Test]
        public void PaymentTransactionRepository_FindById_ShouldReturn()
        {
            // Act
            var payment = this.repository.FindById(RepositoryTestsFixture.PaymentId);

            // Assert
            Assert.IsNotNull(payment);
        }

        [Test]
        public async Task PaymentTransactionRepository_FindById_ShouldReturnNull()
        {
            // Act
            var payment = await this.repository.FindById(default(Guid));

            // Assert
            Assert.IsNull(payment);
        }

        [Test]
        public async Task PaymentTransactionRepository_FindByMerchantId_ShouldReturn()
        {
            var payments = await this.repository.FindByMerchant(RepositoryTestsFixture.MerchantId);

            // Assert
            Assert.IsTrue(payments.Any());
        }

        [Test]
        public async Task PaymentTransactionRepository_FindByMerchantId_ShouldReturnEmpty()
        {
            var payments = await this.repository.FindByMerchant(default(Guid));

            // Assert
            Assert.IsFalse(payments.Any());
        }

        [Test]
        public async Task PaymentTransactionRepository_Update_ShouldUpdate()
        {
            // Arrange
            var payment = await this.repository.FindById(RepositoryTestsFixture.PaymentId);
            payment.OwernName = "UpdatedName";

            // Act
            this.repository.Update(payment);

            var paymentUpdated = await this.repository.FindById(RepositoryTestsFixture.PaymentId);
            Assert.AreEqual(payment.OwernName, paymentUpdated.OwernName);
        }

        private PaymentTransaction GetPaymentTransaction()
        {
            return new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                Amout = 12567.88,
                CurrencyId = 1,
                Currency = new Currency
                {
                    Id = 1
                },
                Merchant = new Merchant
                {
                    Id = Guid.NewGuid()
                },
                Status = new PaymentStatus
                {
                    Id = 1
                }
            };
        }
    }
}
