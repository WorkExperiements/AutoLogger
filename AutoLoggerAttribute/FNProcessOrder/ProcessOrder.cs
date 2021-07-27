using System;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.LogAnalytics;
using Services.LogAnalytics.Models;

namespace FNProcessOrder
{
    public class ProcessOrder
    {
        private readonly IConfiguration _configuration;
        private readonly ILogAnalyticsSrvc _logAnalyticsSrvc;
        public ProcessOrder(IConfiguration configuration)
        {
            _configuration = configuration;
            _logAnalyticsSrvc = new LogAnalyticsSvc(_configuration["LogAnalytics.workspaceId"], _configuration["LogAnalytics.workspaceKey"], _configuration["LogAnalytics.partialLogAnalyticsUrl"]);
        }

        [FunctionName("ProcessOrder")]
        public void Run([ServiceBusTrigger("ordersubmit", Connection = "sb.connString")]string myQueueItem, ILogger log)
        {
            // create artificial 10% chance of failure rate
            if (new Random().Next(1, 11) > 9) { throw new Exception(); }
            var order = JsonConvert.DeserializeObject<Order>(myQueueItem);
            // write into log
            // create the logging payload
            var eventLog = new EventLogEntry()
            {
                EventName = "Order_Detected",
                EventRaiser = "AFN|Process",
                Payload = order.OrderId,
                TimeStamp = DateTime.UtcNow,
                TransactionId = order.TransactionId
            };
            _ = _logAnalyticsSrvc.LogEventAsync(eventLog, "Orders");


            log.LogInformation(myQueueItem);
            log.LogInformation("----");

            // write into db
            // simulates an external dependency that we have no access to
            // Ex: creating an automation in SMC
            BlackBoxService.SpookyOp(_configuration["dbconnectionString"], order);

            
            // write into log
            // create the logging payload
            eventLog = new EventLogEntry()
            {
                EventName = "Order_Processed",
                EventRaiser = "AFN|Process",
                Payload = order.OrderId,
                TimeStamp = DateTime.UtcNow,
                TransactionId = order.TransactionId
            };
            _ = _logAnalyticsSrvc.LogEventAsync(eventLog, "Orders");
        }
    }
}
