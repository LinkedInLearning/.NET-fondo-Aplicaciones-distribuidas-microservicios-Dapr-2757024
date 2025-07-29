using Dapr;
using Microsoft.AspNetCore.Mvc;

namespace FabuRobotics.Production.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionController : ControllerBase
{
    [Topic("pubsub", "orders")]
    public async Task<ActionResult> StartAssembly(OrderCreated orderCreated)
    {
        await Task.Delay(1);
        return Ok();
    }
}

public record OrderCreated(Guid OrderId,
                           string CustomerId,
                           DateTime OrderDate,
                           string Sku,
                           int Quantity);
