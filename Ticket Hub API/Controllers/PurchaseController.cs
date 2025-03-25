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
        // Used to access user secrets/config values
        private readonly IConfiguration _configuration;

        // Inject configuration into the controller
        public PurchaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST: /api/purchase
        [HttpPost]
        public async Task<IActionResult> SubmitPurchase([FromBody] TicketPurchase purchase)
        {
            // Validate incoming data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Define the queue name
            string queueName = "tickethub";

            // Get the connection string from user secrets
            string? connectionString = _configuration["AzureStorageConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                return BadRequest("Connection string is missing.");
            }

            // Connect to the Azure Queue
            var queueClient = new QueueClient(connectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();

            // Convert the object to JSON and send it to the queue
            string message = JsonSerializer.Serialize(purchase);
            await queueClient.SendMessageAsync(message);

            // Return success response
            return Ok("Ticket purchase received and queued.");
        }

    }
}
