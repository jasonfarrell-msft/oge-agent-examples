using GridSimulator.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GridSimulator.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RunSimulationController : ControllerBase
    {
        private readonly ILogger<RunSimulationController> _logger;

        public RunSimulationController(ILogger<RunSimulationController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Run([FromBody] RunSimulationRequestModel request)
        {
            // Log the received request for debugging
            _logger.LogInformation("Received request: {Request}", JsonSerializer.Serialize(request));

            return Ok(request);
        }
    }
}