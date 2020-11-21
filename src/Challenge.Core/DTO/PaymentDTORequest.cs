using System;
using System.ComponentModel.DataAnnotations;

namespace Challenge.Core.DTO
{
    public class PaymentDTORequest
    {
        public Guid Id { get; set; }

        [StringLength(100)]
        public string OwernName { get; set; }

        [StringLength(20, MinimumLength = 16)]
        public string CardNumber { get; set; }

        [StringLength(4, MinimumLength = 4)]
        public string Expire { get; set; }

        [StringLength(4, MinimumLength = 3)]
        public string CVV { get; set; }

        public double Amout { get; set; }

        public int CurrencyId { get; set; }

        public Guid MerchantId { get; set; }
    }
}
