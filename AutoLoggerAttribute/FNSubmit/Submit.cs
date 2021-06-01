using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LoggerAttribute;
using Services.ServiceBus;
using FNSubmit.Models;
using Services.ServiceBus.Models;

namespace FNSubmit
{
    public class Submit
    {
        private readonly IServiceBusSrvc _sbSrvc;
        public Submit(IServiceBusSrvc serviceBusSrvc)
        {
            _sbSrvc = serviceBusSrvc;
        }

        [FunctionName("Submit")]
        [AutoLog]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, 
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string orderId = req.Query["orderId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(requestBody);

            var sbMsg = new OrderMessage() { OrderId = orderId, TransactionId = order.TransactionId };

            string responseMessage = string.IsNullOrEmpty(order.OrderId)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {orderId}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
