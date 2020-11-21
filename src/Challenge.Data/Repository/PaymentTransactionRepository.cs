using System;
using System.Collections.Generic;
using System.Linq;
using Challenge.Core.Model;
using Microsoft.EntityFrameworkCore;
using Challenge.Data.Repository.Interfaces;
using System.Threading.Tasks;

namespace Challenge.Data.Repository
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly ChallengeDbContext dbContext;

        public PaymentTransactionRepository(ChallengeDbContext context)
        {
            this.dbContext = context;
        }

        public Guid Add(PaymentTransaction paymentTransactions)
        {
            paymentTransactions.Status = this.dbContext
                .PaymentStatus
                .FirstOrDefault(p => p.Id == paymentTransactions.Status.Id);

            paymentTransactions.Merchant = this.dbContext
                .Merchant
                .FirstOrDefault(m => m.Id == paymentTransactions.Merchant.Id);

            paymentTransactions.Currency = this.dbContext
                .Currency
                .FirstOrDefault(c => c.Id == paymentTransactions.CurrencyId);

            this.dbContext
                .PaymentTransactions
                .Add(paymentTransactions);

            this.dbContext.SaveChanges();

            return paymentTransactions.Id;
        }

        public async Task<PaymentTransaction> FindById(Guid id)
        {
            return await this.dbContext
                .PaymentTransactions
                .Include(m => m.Merchant)
                .Include(s => s.Status)
                .Include(c => c.Currency)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PaymentTransaction>> FindByMerchant(Guid merchantId)
        {
            return await this.dbContext
                .PaymentTransactions
                .Include(m => m.Merchant)
                .Include(s => s.Status)
                .Include(c => c.Currency)
                .Where(p => p.Merchant.Id == merchantId)
                .ToListAsync();
        }

        public void Update(PaymentTransaction paymentTransactions)
        {
            this.dbContext
                .PaymentTransactions
                .Update(paymentTransactions);

            this.dbContext.SaveChanges();
        }
    }
}
