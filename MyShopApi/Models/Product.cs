using System;
using System.Collections.Generic;

namespace MyShopApi.Models
{
    public partial class Product
    {
        public Product()
        {
            Invoices = new HashSet<Invoice>();
        }

        public int ProductId { get; set; }
        public int? CatgoryId { get; set; }
        public string ProductName { get; set; }
        public int? Qty { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public int? Gst { get; set; }
        public string Image { get; set; }

        public virtual Category Catgory { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
