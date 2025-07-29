using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace FabuRobotics.Production.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController(DaprClient daprClient) : ControllerBase
{
    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult> GetOrderStatus(Guid orderId)
    {
        var status = await daprClient.GetStateAsync<OrderStatus>("statestore", orderId.ToString());

        if (status == null)
        {
            return NotFound();
        }

        return Ok(status);
    }
}
