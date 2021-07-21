using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.LogAnalytics;
using Services.LogAnalytics.Models;
using Services.ServiceBus;
using SubmitApp.Models;
using System;
using System.Threading.Tasks;

namespace SubmitApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogAnalyticsSrvc _logAnalyticsSrvc;
        private readonly IServiceBusSrvc _serviceBus;
        public OrderController(ILogAnalyticsSrvc logAnalyticsSvc, IServiceBusSrvc serviceBusSrvc)
        {
            _logAnalyticsSrvc = logAnalyticsSvc;
            _serviceBus = serviceBusSrvc;
        }

        [HttpPost]
        [Route("Submit")]
        public async Task<IActionResult> Submit(Order order)
        {
            // create the logging payload
            var eventLog = new EventLogEntry()
            {
                EventName = "Order_Submit",
                EventRaiser = "WebApp|Submit",
                Payload = order.OrderId,
                TimeStamp = DateTime.UtcNow,
                TransactionId = Guid.NewGuid().ToString()
            };
            _ = _logAnalyticsSrvc.LogEventAsync(eventLog, "Orders");
            order.TransactionId = eventLog.TransactionId;

            // continue with work, send to service bus
            var serializedOrder  = Newtonsoft.Json.JsonConvert.SerializeObject(order);
            await _serviceBus.SendMessage(serializedOrder);

            return Accepted();
        }
    }
}
