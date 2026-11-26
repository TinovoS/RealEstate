using Identity.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _db;

    public HealthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get()
    {
        var canConnect = await _db.Database.CanConnectAsync();

        if (!canConnect)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { status = "error", database = "unavailable" });
        }

        return Ok(new { status = "ok", database = "available" });
    }
}
