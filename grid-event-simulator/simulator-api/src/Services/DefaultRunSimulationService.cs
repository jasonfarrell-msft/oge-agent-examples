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
        var demandCalcExecutor = new DemandCalculationExecutor(agentFactory.DemandCalculationAgent);
        var outputCalcExecutor = new CalculateOutputExecutor();

        var aggregationExecutor = new ConcurrentAggregationExecutor();

        var workflow = new WorkflowBuilder(startExecutor)
            .AddFanOutEdge(startExecutor, targets: [demandCalcExecutor, outputCalcExecutor])
            .AddFanInEdge(aggregationExecutor, sources: [demandCalcExecutor, outputCalcExecutor])
            .WithOutputFrom(aggregationExecutor)
            .Build();

        StreamingRun run = await InProcessExecution.StreamAsync(workflow, "Create an action plan for the given simulation case");
        await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
        {
            if (evt is WorkflowOutputEvent output)
            {
                Console.WriteLine($"Workflow completed with results:\n{output.Data}");
            }
        }

        return string.Empty;
    }
}

public interface IRunSimulationService
{
    Task<string> RunSimulationAsync(RunSimulationRequestModel request);
}