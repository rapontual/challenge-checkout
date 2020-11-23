using System;

namespace Challenge.Core.DTO
{
    public class PaymentResponseDTO
    {
        public Guid Id { get; set; }

        public string OwernName { get; set; }

        public string CardNumber { get; set; }

        public string Expire { get; set; }

        public double Amout { get; set; }

        public string Currency { get; set; }

        public string Merchant { get; set; }

        public string Status { get; set; }

        public DateTime Date { get; set; }
    }
}
