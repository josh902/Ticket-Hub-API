using Microsoft.AspNetCore.Mvc;
using Ticket_Hub_API.Models;
using Azure.Storage.Queues;
using System.Text.Json;

namespace Ticket_Hub_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PurchaseController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPurchase([FromBody] TicketPurchase purchase)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string queueName = "tickethub";
            string connectionString = _config["AzureStorage:ConnectionString"];

            var queueClient = new QueueClient(connectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();

            string message = JsonSerializer.Serialize(purchase);
            await queueClient.SendMessageAsync(message);

            return Ok("Ticket purchase received and queued.");
        }
    }
}
