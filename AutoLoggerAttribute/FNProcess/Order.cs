using System;
using System.Collections.Generic;
using System.Text;

namespace FNProcess
{
    public class Order
    {
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public Product Product { get; set; }
    }
}
