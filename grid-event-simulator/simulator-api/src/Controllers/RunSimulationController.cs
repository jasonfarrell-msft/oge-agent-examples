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
            
            // Validate that the model binding worked
            if (request == null)
            {
                _logger.LogWarning("Request model is null - model binding failed");
                return BadRequest("Request model is null");
            }

            // Return the bound model to verify values
            return new OkObjectResult(new
            {
                RenewableOutput = request.RenewableOutputInMW,
                TraditionalOutput = request.TraditionalOutputInMW,
                TraditionalRampRate = request.TraditionalRampRateInMin,
                BatteryCharge = request.BatteryChargeInMW,
                BatteryDischargeRate = request.BatteryDischargeInMW,
                ResidentialCustomers = request.NumberOfResidentialCustomers,
                CommercialCustomers = request.NumberOfCommercialCustomers,
                Parameters = request.Parameters
            });
        }
    }
}