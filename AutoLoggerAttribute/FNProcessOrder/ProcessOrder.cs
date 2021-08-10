using System;
using System.Data.SqlClient;
using FNFulfillOrder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
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
            // create artificial 10% chance of failure rate
            if (new Random().Next(1, 11) > 9) { throw new Exception(); }
            var order = JsonConvert.DeserializeObject<Order>(myQueueItem);
            // write into log
            // create the logging payload
            // order detected event


            log.LogInformation(myQueueItem);
            log.LogInformation("----");

            // write into db
            // simulates an external dependency that we have no access to
            // Ex: creating an automation in SMC
            BlackBoxService.SpookyOp(_configuration["dbconnectionString"], order);

            
            // write into log
            // create the logging payload
            // order processed event
            
        }
    }
}
