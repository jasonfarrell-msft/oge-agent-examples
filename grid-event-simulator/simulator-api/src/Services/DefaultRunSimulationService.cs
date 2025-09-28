using GridSimulator.Api.Models;
using Microsoft.Agents.Orchestration;

namespace GridSimulator.Api.Services;

public class DefaultRunSimulationService(IConfiguration configuration, IAgentFactory agentFactory,
    ILogger<DefaultRunSimulationService> logger) : IRunSimulationService
{    
    public async Task<string> RunSimulationAsync(RunSimulationRequestModel request)
    {
        var orchestration = new SequentialOrchestration(agentFactory.AllAgents)
        {
            ResponseCallback = (response) =>
            {
                logger.LogInformation("Orchestration response: {Response}", response);
                return ValueTask.CompletedTask;
            }
        };

        var input = Prompts.GetOutputReductionSimulationInput(request);
        var responseResult = await orchestration.RunAsync(input);

        return string.Empty;

        // reference: https://github.com/microsoft/agent-framework/blob/main/dotnet/samples/GettingStarted/AgentOrchestration/Orchestration/SequentialOrchestration_Multi_Agent.cs
    }
}

public interface IRunSimulationService
{
    Task<string> RunSimulationAsync(RunSimulationRequestModel request);
}