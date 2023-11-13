using StoreSaleClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StoreSaleClient.ViewModels
{
    public class ProductViewModels : INotifyPropertyChanged
    {
        private string _ProductImg;
        public string ProductImg
        {
            get { return this._ProductImg; }
            set
            {
                if (_ProductImg != value)
                {
                    _ProductImg = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(ProductImg)));
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ProductViewModels()
        {

        }

        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }
        public DateTime? WarehouseArrivalDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CategoryId { get; set; }
        public int? Quantity { get; set; }
        public void setValue(Product hold)
        {
            this.ProductImg = hold.ProductImg;
            this.ProductId = hold.ProductId;
            this.ProductName = hold.ProductName;
            this.Price = hold.Price;
            this.WarehouseArrivalDate = hold.WarehouseArrivalDate;
            this.ExpirationDate = hold.ExpirationDate;
            this.CategoryId = hold.CategoryId;
            this.Quantity = hold.Quantity;
        }
    }
}
