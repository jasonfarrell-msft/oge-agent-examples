using GridSimulator.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GridSimulator.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RunSimulationController : ControllerBase
    {
        [HttpPost]
        public IActionResult Run([FromBody] RunSimulationRequestModel request)
        {
            return new OkObjectResult(request);
        }
    }
}