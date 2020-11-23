using System;

namespace Challenge.Core.DTO
{
    public class PaymentCreateResponseDTO
    {
        public Guid TransactionId { get; set; }

        public int StatusId { get; set; }

        public string Status { get; set; }
    }
}
