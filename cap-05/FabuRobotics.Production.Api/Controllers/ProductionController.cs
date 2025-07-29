using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace FabuRobotics.Production.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionController(DaprClient daprClient) : ControllerBase
{
    [Topic("pubsub", "orders")]
    public async Task<ActionResult> StartAssembly(OrderCreated orderCreated)
    {
        System.Timers.Timer timer = new System.Timers.Timer(10000);
        OrderStatus currentStatus = OrderStatus.Preparing;

        timer.Elapsed += async (sender, e) =>
        {
            await daprClient.SaveStateAsync("statestore",
                                            orderCreated.OrderId.ToString(),
                                            currentStatus);

            (currentStatus, bool stopTimer) = currentStatus switch
            {
                OrderStatus.Preparing => (OrderStatus.Assembling, false),
                OrderStatus.Assembling => (OrderStatus.Assembled, false),
                OrderStatus.Assembled => (OrderStatus.Shipped, false),
                OrderStatus.Shipped => (currentStatus, true),
                _ => (currentStatus, false)
            };

            if (stopTimer)
            {
                timer.Stop();
            }
        };

        timer.Start();

        return Ok();
    }
}

public record OrderCreated(Guid OrderId,
                           string CustomerId,
                           DateTime OrderDate,
                           string Sku,
                           int Quantity);

public enum OrderStatus
{
    Preparing,
    Assembling,
    Assembled,
    Shipped
}