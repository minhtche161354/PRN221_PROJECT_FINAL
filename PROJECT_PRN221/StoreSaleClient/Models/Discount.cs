using System;
using System.Collections.Generic;

namespace StoreSaleClient.Models
{
    public partial class Discount
    {
        public Discount()
        {
            Bills = new HashSet<Bill>();
        }

        public int DiscountId { get; set; }
        public string? DiscountName { get; set; }
        public decimal? DiscountPercentage { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
    }
}
