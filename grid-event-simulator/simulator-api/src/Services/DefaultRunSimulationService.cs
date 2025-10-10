using GridSimulator.Api.Executors;
using GridSimulator.Api.Models;
using Microsoft.Agents.AI.Workflows;

namespace GridSimulator.Api.Services;

public class DefaultRunSimulationService(IConfiguration configuration, IAgentFactory agentFactory,
    IServiceProvider serviceProvider) : IRunSimulationService
{
    public async Task<string> RunSimulationAsync(RunSimulationRequestModel request)
    {
        var demandCalcExecutor = new DemandCalculationExecutor(agentFactory.DemandCalculationAgent,
            serviceProvider.GetRequiredService<ILogger<DemandCalculationExecutor>>());
        var outputCalcExecutor = new CalculateOutputExecutor(serviceProvider.GetRequiredService<ILogger<CalculateOutputExecutor>>());
        var gridAnalysisExecutor = new GridAnalysisAgentExecutor(agentFactory.GridAnalysisAgent);
        var actionPlanExecutor = new ActionPlanAgentExecutor(agentFactory.ActionPlanAgent);

        var workflow = new WorkflowBuilder(demandCalcExecutor)
            .AddEdge(demandCalcExecutor, outputCalcExecutor)
            .AddEdge(outputCalcExecutor, gridAnalysisExecutor)
            .AddEdge(gridAnalysisExecutor, actionPlanExecutor)
            .WithOutputFrom(actionPlanExecutor)
            .Build();

        var run = await InProcessExecution.StreamAsync(workflow, input: request);
        await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
        {
            if (evt is WorkflowOutputEvent workflowComplete)
            {
                return workflowComplete.Data.ToString() ?? throw new Exception("Did not get a Workflow result.");
            }
        }

        throw new Exception("Workflow did not complete successfully.");
    }
}

public interface IRunSimulationService
{
    Task<string> RunSimulationAsync(RunSimulationRequestModel request);
}