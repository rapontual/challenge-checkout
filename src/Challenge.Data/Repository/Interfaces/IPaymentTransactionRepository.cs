using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Challenge.Core.Model;

namespace Challenge.Data.Repository.Interfaces
{
    public interface IPaymentTransactionRepository
    {
        Guid Add(PaymentTransaction paymentTransactions);

        void Update(PaymentTransaction paymentTransactions);

        Task<PaymentTransaction> FindById(Guid id);

        Task<IEnumerable<PaymentTransaction>> FindByMerchant(Guid merchantId);
    }
}
