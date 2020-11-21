using Challenge.Core.Model;
using Challenge.Data.Repository.Interfaces;
using Challenge.Service.BankClient;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace Challenge.Service
{
    public class ApprovePaymentService : IApprovePaymentService
    {
        private readonly IBankApprovalService bankApprovalService;
        private readonly IPaymentTransactionRepository repository;
        private readonly ILogger<ApprovePaymentService> logger;

        public ApprovePaymentService(
            IBankApprovalService bankApprovalService, 
            IPaymentTransactionRepository repository, 
            ILogger<ApprovePaymentService> logger)
        {
            this.bankApprovalService = bankApprovalService;
            this.repository = repository;
            this.logger = logger;
        }


        public PaymentTransaction ApprovePayment(PaymentTransaction payment)
        {
            var request = new BankApprovalRequest
            {
                Amout = payment.Amout,
                CardNumber = long.Parse(payment.CardNumber),
                CurrencyId = payment.CurrencyId,
                CVV = int.Parse(payment.CVV),
                Expire = payment.Expire,
                MerchantId = payment.Merchant.Id,
                OwernName = payment.OwernName
            };

            int statusId = 3;

            try
            {
                var response = this.bankApprovalService.Approve(request);

                // Simple parse here, but it should be mapped
                statusId = response.Status == BankAporovalStatus.Approved
                    ? 1
                    : 2;

                payment.BankTransactionId = response.TransactionId;

                this.logger.LogInformation(
                    "BankApprovalServiceResponse for Merchant {0}, OwnerName '{1}', Amout {2}, Status {3}",
                     request.MerchantId,
                     payment.OwernName,
                     payment.Amout,
                     response.Status);

            }
            catch(Exception ex)
            {
                // Set error payment
                statusId = 4;

                this.logger.LogError(
                    ex,
                    "BankApprovalServiceError for Merchant {0}, OwnerName '{1}', Amout {2}",
                    request.MerchantId,
                    payment.OwernName,
                    payment.Amout);
            }
                       

            payment.Status = new PaymentStatus
            {
                Id = statusId
            };

            payment.Date = DateTime.Now;
            payment.Id = repository.Add(payment);
            
            return payment;
        }

        public string ValidateCard(string cardNumber, string expire, string cvv, string owernName)
        {
            // Simple validation, could also validate CVV

            StringBuilder sb = new StringBuilder();

            if (cardNumber.Length < 16)
            {
                sb.AppendLine("Invalid CardNumber.");
            }
            
            if (expire.Length != 4)
            {
                sb.AppendLine("Invalid Expiration.");
            }

            if (cvv.Length < 3 || cvv.Length > 4)
            {
                sb.AppendLine("Invalid CVV.");
            }

            if (string.IsNullOrWhiteSpace(owernName))
            {
                sb.AppendLine("Invalid Owner Name.");
            }

            return sb.ToString();
        }
    }
}
