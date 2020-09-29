using System;
using System.Collections.Generic;

namespace MyShopApi.Models
{
    public partial class Invoice
    {
        public int InvoiceId { get; set; }
        public int? ProductId { get; set; }
        public int? Qty { get; set; }
        public double? Price { get; set; }
        public int? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}
