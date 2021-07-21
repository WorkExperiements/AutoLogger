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
            var order = JsonConvert.DeserializeObject<Order>(myQueueItem);
            log.LogInformation(myQueueItem);
            log.LogInformation("----");

            // write into db
            SqlConnection cnn = new SqlConnection(_configuration["dbconnectionString"]);

            cnn.Open();

            var command = cnn.CreateCommand();
            command.CommandText = $"INSERT INTO [dbo].[Order] (OrderId, TransactionId, ProductName, ProductId, Status, LastUpdated) VALUES ('{order.OrderId}', '{order.TransactionId}', '{order.Product.Name}', '{order.Product.ID}', 'Processed', '{DateTime.Now}')";
            var count = command.ExecuteNonQuery();
            cnn.Close();


            // write into log
            // create the logging payload
            var eventLog = new EventLogEntry()
            {
                EventName = "Order_Process",
                EventRaiser = "AFN|Process",
                Payload = order.OrderId,
                TimeStamp = DateTime.UtcNow,
                TransactionId = order.TransactionId
            };
            _ = _logAnalyticsSrvc.LogEventAsync(eventLog, "Orders");
        }
    }
}
