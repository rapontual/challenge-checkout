using System;

namespace Challenge.Service.BankClient
{
    public class BankApprovalRequest
    {
        public string OwernName { get; set; }

        public long CardNumber { get; set; }

        public string Expire { get; set; }

        public int CVV { get; set; }

        public double Amout { get; set; }

        public int CurrencyId { get; set; }

        public Guid MerchantId { get; set; }
    }
}
