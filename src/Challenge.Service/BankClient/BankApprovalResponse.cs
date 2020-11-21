using System;

namespace Challenge.Service.BankClient
{
    public class BankApprovalResponse
    {
        public Guid TransactionId { get; set; }

        public BankAporovalStatus Status { get; set; }
    }
}
