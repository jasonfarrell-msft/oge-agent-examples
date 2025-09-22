using Microsoft.AspNetCore.Mvc;

namespace GridSimulator.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RunSimulationController : ControllerBase
    {
        [HttpPost]
        public IActionResult Run()
        {
            return new OkObjectResult("Hello World");
        }
    }
}