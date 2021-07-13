using LoggerAttribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SubmitApp.Models;

namespace SubmitApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [AutoLog]
        [HttpPost]
        [Route("Submit")]
        public IActionResult Submit(Order order)
        {

            return Accepted();
        }
    }
}
