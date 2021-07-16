using System;
using System.IO;
using System.Threading.Tasks;
using FNSubmit.Models;
using LoggerAttribute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FNProcess
{
    public static class FNProcess
    {
        [FunctionName("FNProcess")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation($"Order process function triggered!");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation(requestBody);
            //var order = JsonConvert.DeserializeObject<Order>(requestBody);
            return new OkObjectResult(requestBody);
        }
    }
}
