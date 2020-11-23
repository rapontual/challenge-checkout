using System;

namespace Challenge.Service.BankClient
{
    public class BrankApprovalServiceFake : IBankApprovalService
    {
        public BankApprovalResponse Approve(BankApprovalRequest request)
        {
            return new BankApprovalResponse
            {
                TransactionId = Guid.NewGuid(),
                Status = this.GetStatus(request)
            };
        }

        private BankAporovalStatus GetStatus(BankApprovalRequest request)
        {
            return (request.MerchantId == default(Guid) ||
                   request.CardNumber == default ||
                   request.CVV == default ||
                   request.Expire == default ||
                   string.IsNullOrWhiteSpace(request.OwernName))
                   ? BankAporovalStatus.Denied
                   : BankAporovalStatus.Approved;
        }
    }
}
