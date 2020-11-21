using System;
using System.Collections.Generic;
using System.Text;

namespace Challenge.Core.DTO
{
    public class PaymentResponseDTO
    {
        public Guid TransactionId { get; set; }

        public int StatusId { get; set; }

        public string Status { get; set; }
    }
}
