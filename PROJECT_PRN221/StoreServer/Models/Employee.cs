using System;
using System.Collections.Generic;

namespace StoreServer.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Bills = new HashSet<Bill>();
        }

        public int EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Account { get; set; }
        public string? Password { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
    }
}
