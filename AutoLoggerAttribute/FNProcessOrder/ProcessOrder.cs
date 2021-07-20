using System;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FNProcessOrder
{
    public class ProcessOrder
    {
        private readonly IConfiguration _configuration;
        public ProcessOrder(IConfiguration configuration)
        {
            _configuration = configuration;
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
            command.CommandText = $"INSERT INTO [dbo].[Order] (OrderId, TransactionId, ProductName, ProductId, Status) VALUES ('{order.OrderId}', '{order.TransactionId}', '{order.Product.Name}', '{order.Product.ID}', 'Processed')";
            var count = command.ExecuteNonQuery();
            cnn.Close();
        }
    }
}
