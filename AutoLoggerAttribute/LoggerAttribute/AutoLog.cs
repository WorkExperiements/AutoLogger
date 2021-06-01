using Microsoft.Azure.WebJobs.Host;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Services.LogAnalytics;
using Microsoft.Extensions.Configuration;
using Services.LogAnalytics.Models;
using Newtonsoft.Json;
using FNSubmit.Models;

namespace LoggerAttribute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoLog : FunctionInvocationFilterAttribute
    {
        private IConfigurationRoot _config { get; set; }

        public override Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            var args = executingContext.Arguments.ToList();
            var context = executingContext.Arguments.Where(arg => arg.Value.GetType().Equals(typeof(Microsoft.Azure.WebJobs.ExecutionContext)))
                .Select(arg => arg.Value)
                .FirstOrDefault() as Microsoft.Azure.WebJobs.ExecutionContext;

            // create configuration
            _config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // instantiate service
            var logAnalyticsSrvc = new LogAnalyticsSvc();
            logAnalyticsSrvc.Init(_config["LogAnalytics.workspaceId"], _config["LogAnalytics.workspaceKey"], _config["LogAnalytics.partialLogAnalyticsUrl"]);

            // create payload
            var eventRaiser = executingContext.FunctionName;
            var workItem = executingContext.Arguments.First().Value as HttpRequest;
            var requestBody = string.Empty;
            using (StreamReader streamReader = new StreamReader(workItem.Body))
            {
                requestBody = streamReader.ReadToEndAsync().Result;
            }

            var order = JsonConvert.DeserializeObject<Order>(requestBody);
            //var requestBody = result.ToString();
            var queryStrings = workItem.Query;

            var eventLogEntry = new EventLogEntry() {
                EventName = "Order_Submit",
                EventRaiser = eventRaiser,
                Payload = requestBody,
                TransactionId = order.TransactionId,
                TimeStamp = DateTime.UtcNow
            };

            //send event log
            var resp = logAnalyticsSrvc.LogEventAsync(eventLogEntry, _config["LogAnalytics.customLogName"]).Result;

            return base.OnExecutingAsync(executingContext, cancellationToken);
        }
    }
}
