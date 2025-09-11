using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Farrellsoft.Examples.Agents.MultiAgent.Services;

namespace Farrellsoft.Examples.Agents.MultiAgent.Functions
{
    public class RunAnalysis(ILogger<RunAnalysis> logger, IAgentService agentService)
    {
        [Function("run_analysis")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "execute")] HttpRequest req)
        {
            logger.LogInformation("Invoking RunAnalysis function.");

            var analysisResult = await agentService.ExecuteAnalysis();
            if (analysisResult == null)
            {
                logger.LogError("AnalysisResult is null");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new OkObjectResult(analysisResult);
        }
    }
}
