using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challenge.Core.Model
{
    public class Merchant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public string Name { get; set; }

        public string Login { get; set; }

        [Encrypted]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        public ICollection<PaymentTransaction> Payments { get; set; }
    }
}
