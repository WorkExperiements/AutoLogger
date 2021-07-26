using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Services.LogAnalytics;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace FNFulfillOrder
{
    public class FNFulfill
    {
        private readonly IConfiguration _configuration;
        private readonly ILogAnalyticsSrvc _logAnalyticsSrvc;
        public FNFulfill(IConfiguration configuration)
        {
            _configuration = configuration;
            _logAnalyticsSrvc = new LogAnalyticsSvc(_configuration["LogAnalytics.workspaceId"], _configuration["LogAnalytics.workspaceKey"], _configuration["LogAnalytics.partialLogAnalyticsUrl"]);
        }

        [FunctionName("Fulfill")]
        public  IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // update orders to completed
            DBHelper.UpdateOrders(_configuration["dbconnectionString"]); 

            return new OkObjectResult(null);
        }
    }
}
