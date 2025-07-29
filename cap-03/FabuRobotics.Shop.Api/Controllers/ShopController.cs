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

        return Ok();
    }
}

public record NewOrder(string CustomerId, string Sku, int Quantity);

public record InventoryAvailable(string Sku, bool Available);
