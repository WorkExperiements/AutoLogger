using System;
using System.Collections.Generic;
using System.Text;

namespace Services.LogAnalytics.Models
{
    public class EventLogEntry
    {
        public string TransactionId { get; set; }
        public string EventRaiser { get; set; }
        public string EventName { get; set; }
        public string Payload { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
