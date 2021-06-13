using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableEntities
{
    public static class CounterFunctions
    {
        [FunctionName("get-counter")]
        public static async Task<IActionResult> GetCounter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "counter")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation("Getting the counter");
            EntityId entityId = new EntityId(nameof(Counter), "arch-next-demo");
            EntityStateResponse<Counter> counter = await client.ReadEntityStateAsync<Counter>(entityId);
            
            return new OkObjectResult(counter.EntityState.CountValue);
        }

        [FunctionName("set-counter")]
        public static async Task<IActionResult> SetCounter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "counter")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation("Setting the counter");

            EntityId entityId = new EntityId(nameof(Counter), "arch-next-demo");
            await client.SignalEntityAsync(entityId, nameof(Counter.Increment));

            return new OkResult();
        }

        [FunctionName("reset-counter")]
        public static async Task<IActionResult> ResetCounter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "reset")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation("Counter is reset");

            EntityId entityId = new EntityId(nameof(Counter), "arch-next-demo");
            await client.SignalEntityAsync(entityId, nameof(Counter.Reset));

            return new OkResult();
        }
    }
}
