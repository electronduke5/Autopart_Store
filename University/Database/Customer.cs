using System;
using System.Collections.Generic;

#nullable disable

namespace University.Database
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        public int IdCustomer { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
