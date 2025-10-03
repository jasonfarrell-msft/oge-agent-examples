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
        var demandCalcExecutor = new DemandCalculationExecutor(agentFactory.DemandCalculationAgent);
        var outputCalcExecutor = new CalculateOutputExecutor();
        var gridAnalysisExecutor = new GridAnalysisAgentExecutor(agentFactory.GridAnalysisAgent);
        var actionPlanAgent = agentFactory.ActionPlanAgent;

        var workflow = new WorkflowBuilder(demandCalcExecutor)
            .AddEdge(demandCalcExecutor, outputCalcExecutor)
            .AddEdge(outputCalcExecutor, gridAnalysisExecutor)
            //.AddEdge(gridAnalysisAgent, actionPlanAgent)
            .WithOutputFrom(gridAnalysisExecutor)
            .Build();

        var run = await InProcessExecution.StreamAsync(workflow, input: request);
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