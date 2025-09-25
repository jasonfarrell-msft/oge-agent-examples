using GridSimulator.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using GridSimulator.Api.Services;

namespace GridSimulator.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RunSimulationController(
        ILogger<RunSimulationController> logger,
        IRunSimulationService runSimulationService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Run([FromBody] RunSimulationRequestModel request)
        {
            // Log the received request for debugging
            logger.LogInformation("Received request: {Request}", JsonSerializer.Serialize(request));
            
            // run the simulation with the given parameters
            var result = await runSimulationService.RunSimulationAsync(request);

            return Ok(result);
        }
    }
}