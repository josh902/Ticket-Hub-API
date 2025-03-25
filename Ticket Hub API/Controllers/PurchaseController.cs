using Microsoft.AspNetCore.Mvc;
using Ticket_Hub_API.Models;
using Azure.Storage.Queues;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ticket_Hub_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SubmitPurchase([FromBody] TicketPurchase purchase)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set up Azure Queue
            string queueName = "tickethub";
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=nscc0468743storageacct;AccountKey=07DplxGAypPorxXJHXwflTAaXcdaB5Z2XJpeQ9uQBr/p9BMMlKbxdNcDpY25yQ1vsMnvch6MI2AS+AStIkYJiQ==;EndpointSuffix=core.windows.net"; // Replace this make sure to i ahe the azure thing

            var queueClient = new QueueClient(connectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();

            // Convert object to JSON and send to queue
            string message = JsonSerializer.Serialize(purchase);
            await queueClient.SendMessageAsync(message);

            return Ok("Ticket purchase received and queued.");
        }
    }
}
