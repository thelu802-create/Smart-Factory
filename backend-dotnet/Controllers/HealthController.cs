using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Data;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController(SampleDataService data, SqlServerConnectionFactory sqlServer) : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new { status = "ok", dataSource = data.GetDataSourceStatus() });
    }

    /// <summary>
    /// Verifies backend connectivity to the SmartFactory SQL Server database.
    /// tableCount = 0 is expected until the code-first migrations create the schema.
    /// </summary>
    [HttpGet("database")]
    public IActionResult GetDatabaseHealth()
    {
        var probe = sqlServer.Probe();
        var payload = new
        {
            status = probe.Connected ? "ok" : "error",
            provider = "SqlServer",
            probe.Connected,
            probe.Server,
            probe.Database,
            serverVersion = probe.ServerVersion,
            probe.TableCount,
            probe.Error
        };

        return probe.Connected
            ? Ok(payload)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, payload);
    }
}
