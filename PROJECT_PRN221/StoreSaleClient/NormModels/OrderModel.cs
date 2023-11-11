using StoreSaleClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSaleClient.NormModels
{
    public class OrderModel
    {
        public Product? Product { get; set; }
        public int quantity { get; set; }
    }
}
