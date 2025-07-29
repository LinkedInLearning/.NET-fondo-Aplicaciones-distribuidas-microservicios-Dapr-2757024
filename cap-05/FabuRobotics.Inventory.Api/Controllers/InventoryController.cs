using Microsoft.AspNetCore.Mvc;

namespace FabuRobotics.Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly Dictionary<string, bool> availability = new()
    {
        { "FB-01", true },
        { "FB-02", true },
        { "FB-03", false }
    };

    [HttpGet("{sku}/availability")]
    public async Task<ActionResult> GetAvailability(string sku)
    {
        await Task.Delay(1);

        var isAvailable = availability.First(availability => availability.Key.Equals(sku, StringComparison.OrdinalIgnoreCase)).Value;

        return Ok(new { Sku = sku, Available = isAvailable });
    }
}