using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FNProcess
{
    public class FNProcess
    {
        private readonly IConfiguration _config;
        public FNProcess(IConfiguration config)
        {
            _config = config;
        }

        [FunctionName("FNProcess")]
        public void Run([ServiceBusTrigger("ordersubmit", Connection = "sb.connString")]
    string myQueueItem,
    Int32 deliveryCount,
    DateTime enqueuedTimeUtc,
    string messageId,
    ILogger log)
        {
            var order = JsonConvert.DeserializeObject<Order>(myQueueItem);
            log.LogInformation(myQueueItem);
            log.LogInformation("----");

            // write into db
            SqlConnection cnn = new SqlConnection(_config["dbconnectionString"]);

            cnn.Open();

            var command = cnn.CreateCommand();
            command.CommandText = $"INSERT INTO order (OrderId, TransactionId, ProductName, ProductId, Status) VALUES ('{order.OrderId}', '{order.TransactionId}', '{order.Product.Name}', '{order.Product.ID}', 'Processed')";
            var count = command.ExecuteNonQuery();
            cnn.Close();

        }
    }
}
