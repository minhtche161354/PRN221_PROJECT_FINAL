using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StoreSaleClient.Models
{
    public partial class Product : INotifyPropertyChanged
    {
        private Product _p;
        public Product _Product
        {
            get { return this._p; }
            set
            {
                if (_p != value)
                {
                    _p = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(_Product)));
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
