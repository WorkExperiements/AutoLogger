using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        
        private readonly IServiceBusSrvc _serviceBus;
        public OrderController(IServiceBusSrvc serviceBusSrvc)
        {
            _serviceBus = serviceBusSrvc;
        }

        [HttpPost]
        [Route("Submit")]
        public async Task<IActionResult> Submit(Order order)
        {
            // create the logging payload
            order.TransactionId = Guid.NewGuid().ToString();

            // create artificial 10% chance of failure rate
            var currChance = new Random().Next(1, 11);
            if ( currChance > 9) { throw new Exception(); }

            // continue with work, send to service bus
            var serializedOrder  = Newtonsoft.Json.JsonConvert.SerializeObject(order);
            await _serviceBus.SendMessage(serializedOrder);

            return Accepted();
        }
    }
}
