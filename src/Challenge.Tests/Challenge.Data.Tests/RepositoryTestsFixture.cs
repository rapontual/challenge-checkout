using System;
using System.Linq;
using Challenge.Core.Model;
using Challenge.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Challenge.Tests.Data
{
    [SetUpFixture]
    public class RepositoryTestsFixture
    {
        public static Guid MerchantId { get; set; }

        public static Guid PaymentId { get; set; }

        public static ChallengeDbContext DbContext { get; set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            MerchantId = Guid.NewGuid();

            var options = new DbContextOptionsBuilder<ChallengeDbContext>()
                .UseInMemoryDatabase(databaseName: "ChallengeDbContext")
                .Options;

            // DbContext data setup
            var merchant = new Merchant { Id = MerchantId, Name = "MerchantTest" };
            var currency = new Currency { Id = 1, Name = "CurrencyTest" };
            var paymentStatus1 = new PaymentStatus { Id = 1, StatusDescription = "Test1" };
            var paymentStatus2 = new PaymentStatus { Id = 2, StatusDescription = "Test2" };

            DbContext = new ChallengeDbContext(options);

            DbContext.PaymentStatus.Add(paymentStatus1);
            DbContext.PaymentStatus.Add(paymentStatus2);
            DbContext.Merchant.Add(merchant);
            DbContext.Currency.Add(currency);

            DbContext.PaymentTransactions.Add(new PaymentTransaction
            {
                Amout = 1200.22,
                BankTransactionId = Guid.NewGuid(),
                CardNumber = "1111222233334444",
                CurrencyId = 1,
                Currency = currency,
                CVV = "1234",
                Date = DateTime.Now,
                Expire = "1230",
                Merchant = merchant,
                OwernName = "Test Owner",
                Status = paymentStatus1
            });

            DbContext.SaveChanges();

            PaymentId = DbContext.PaymentTransactions.FirstOrDefault(s => s.Merchant.Id == merchant.Id).Id;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            DbContext.Dispose();
        }
    }
}
