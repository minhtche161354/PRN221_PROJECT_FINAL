using System;
using System.Collections.Generic;

namespace StoreServer.Models
{
    public partial class Bill
    {
        public Bill()
        {
            BillDetails = new HashSet<BillDetail>();
        }

        public int BillId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? BillDate { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? DiscountId { get; set; }
        public decimal? TotalWorth { get; set; }

        public virtual Discount? Discount { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual PaymentMethod? PaymentMethod { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
