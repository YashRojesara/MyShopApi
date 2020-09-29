using System;
using System.Collections.Generic;

namespace MyShopApi.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Invoices = new HashSet<Invoice>();
        }

        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public double? Total { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
