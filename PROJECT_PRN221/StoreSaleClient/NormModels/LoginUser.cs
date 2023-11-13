using StoreSaleClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSaleClient.NormModels
{
    public class LoginUser
    {
        public string UserName { get; set;}
        public int? Role { get; set; }
        public Employee? Details { get; set; }
        public string Password { get; set; }
    }
}
