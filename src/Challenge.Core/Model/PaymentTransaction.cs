using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challenge.Core.Model
{
    public class PaymentTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        [Encrypted]
        [StringLength(100)]  // this is size is due to encryption
        public string OwernName { get; set; }

        [Encrypted]
        [StringLength(100)]
        public string CardNumber { get; set; }

        [Encrypted]
        [StringLength(100)]
        public string Expire { get; set; }

        [Encrypted]
        [StringLength(100)]
        public string CVV { get; set; }

        public double Amout { get; set; }

        public int CurrencyId { get; set; }

        public Guid BankTransactionId { get; set; }

        public PaymentStatus Status { get; set; }

        public Merchant Merchant { get; set; }

        public Currency Currency { get; set; }
    }
}
