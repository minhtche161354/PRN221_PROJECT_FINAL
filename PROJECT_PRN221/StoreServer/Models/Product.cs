using System;
using System.Collections.Generic;

namespace StoreServer.Models
{
    public partial class Product
    {
        public Product()
        {
            BillDetails = new HashSet<BillDetail>();
        }

        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImg { get; set; }
        public decimal? Price { get; set; }
        public DateTime? WarehouseArrivalDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CategoryId { get; set; }
        public int? Quantity { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
