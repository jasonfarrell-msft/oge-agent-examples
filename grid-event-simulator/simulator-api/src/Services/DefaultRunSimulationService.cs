using GridSimulator.Api.Executors;
using GridSimulator.Api.Models;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace GridSimulator.Api.Services;

public class DefaultRunSimulationService(IConfiguration configuration, IAgentFactory agentFactory,
    ILogger<DefaultRunSimulationService> logger) : IRunSimulationService
{
    public async Task<string> RunSimulationAsync(RunSimulationRequestModel request)
    {
        var startExecutor = new ConcurrentStartExecutor();
        var aggregationExecutor = new ConcurrentAggregationExecutor();

        var workflow = new WorkflowBuilder(startExecutor)
            .AddFanOutEdge

        return string.Empty;
    }
}

public interface IRunSimulationService
{
    Task<string> RunSimulationAsync(RunSimulationRequestModel request);
}