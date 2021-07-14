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
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http.Internal;

namespace LoggerAttribute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoLog : FunctionInvocationFilterAttribute
    {
        private IConfigurationRoot _config { get; set; }

        private EventLogEntry CreateLogEntry(string inEventRaiser, HttpRequest inWorkItem )
        {
            // create payload
            var eventRaiser = inEventRaiser;
            var workItem = inWorkItem;
            var requestBody = string.Empty;
            using (StreamReader streamReader = new StreamReader(workItem.Body))
            {
                requestBody = streamReader.ReadToEndAsync().Result;
            }

            var order = JsonConvert.DeserializeObject<Order>(requestBody);
            //var requestBody = result.ToString();
            var queryStrings = workItem.Query;

            var eventLogEntry = new EventLogEntry()
            {
                EventName = "Order_Submit",
                EventRaiser = eventRaiser,
                Payload = requestBody,
                TransactionId = order.TransactionId,
                TimeStamp = DateTime.UtcNow
            };
            return eventLogEntry;
        }

        public override Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            var args = executingContext.Arguments.ToList();
            var context = executingContext.Arguments.Where(arg => arg.Value.GetType().Equals(typeof(Microsoft.Azure.WebJobs.ExecutionContext)))
                .Select(arg => arg.Value)
                .FirstOrDefault() as Microsoft.Azure.WebJobs.ExecutionContext;

            // create configuration
            _config = OptionsHelper.GetConfigs(context.FunctionAppDirectory);

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

    public class ControllerAutoLog: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext executingContext)
        {
            base.OnActionExecuting(executingContext);

            var config = OptionsHelper.GetConfigs(Environment.CurrentDirectory);

            // instantiate service
            var logAnalyticsSrvc = new LogAnalyticsSvc();
            logAnalyticsSrvc.Init(config["LogAnalytics.workspaceId"], config["LogAnalytics.workspaceKey"], config["LogAnalytics.partialLogAnalyticsUrl"]);

            // create payload
            var eventRaiser = $"{executingContext.Controller}|{executingContext.ActionDescriptor.DisplayName}";
            string requestBodyStr = ReadBodyAsString(executingContext.HttpContext.Request);
            var order = JsonConvert.DeserializeObject<Order>(requestBodyStr);
            
            var eventLogEntry = new EventLogEntry()
            {
                EventName = "Order_Submit",
                EventRaiser = eventRaiser,
                Payload = JsonConvert.SerializeObject(order),
                TransactionId = order?.TransactionId ?? string.Empty,
                TimeStamp = DateTime.UtcNow
            };

            //send event log
            var resp = logAnalyticsSrvc.LogEventAsync(eventLogEntry, config["LogAnalytics.customLogName"]).Result;

        }

        private string ReadBodyAsString(HttpRequest request)
        {
            var initialBody = request.Body; // Workaround

            try
            {
                request.EnableBuffering();

                using (StreamReader reader = new StreamReader(request.Body))
                {
                    string text = reader.ReadToEndAsync().Result;
                    return text;
                }
            }
            finally
            {
                // Workaround so MVC action will be able to read body as well
                request.Body = initialBody;
            }
            return string.Empty;
        }
    }

    public static class OptionsHelper
    {
        public static IConfigurationRoot GetConfigs(string basePath)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(basePath)
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();
            return config;
        }
    }
}
