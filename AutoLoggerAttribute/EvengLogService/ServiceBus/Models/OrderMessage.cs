using System;
using System.Collections.Generic;
using System.Text;

namespace Services.ServiceBus.Models
{
    public class OrderMessage
    {
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
    }
}
