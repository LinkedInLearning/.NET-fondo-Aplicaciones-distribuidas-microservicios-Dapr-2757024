using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace FabuRobotics.Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopController(DaprClient daprClient) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> PlaceOrder([FromBody] NewOrder newOrder)
    {
        var result = await daprClient.InvokeMethodAsync<InventoryAvailable>(HttpMethod.Get,
            "inventory-api", $"api/inventory/{newOrder.Sku}/availability");

        if (!result.Available)
        {
            return BadRequest("Not enough stock available.");
        }

        var orderCreated = new OrderCreated(Guid.NewGuid(),
                                            newOrder.CustomerId,
                                            DateTime.UtcNow,
                                            newOrder.Sku,
                                            newOrder.Quantity);
        await daprClient.PublishEventAsync("pubsub", "orders", orderCreated);

        return Ok();
    }
}

public record NewOrder(string CustomerId, string Sku, int Quantity);

public record InventoryAvailable(string Sku, bool Available);

public record OrderCreated(Guid OrderId,
                           string CustomerId,
                           DateTime OrderDate,
                           string Sku,
                           int Quantity);






