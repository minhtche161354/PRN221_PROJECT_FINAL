using System;
using System.Collections.Generic;

namespace StoreServer.Models
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            Bills = new HashSet<Bill>();
        }

        public int PaymentMethodId { get; set; }
        public string? PaymentMethodName { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
    }
}
