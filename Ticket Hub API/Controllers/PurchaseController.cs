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
        private readonly IConfiguration _configuration;

        public PurchaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPurchase([FromBody] TicketPurchase purchase)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string queueName = "tickethub";
            string? connectionString = _configuration["AzureStorageConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                return BadRequest("Connection string is missing.");
            }

            var queueClient = new QueueClient(connectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();

            string message = JsonSerializer.Serialize(purchase);
            await queueClient.SendMessageAsync(message);

            return Ok("Ticket purchase received and queued.");
        }
    }
}
