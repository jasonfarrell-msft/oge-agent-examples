using GridSimulator.Api.Executors;
using GridSimulator.Api.Models;
using Microsoft.Agents.Workflows;
using Microsoft.Extensions.AI;

namespace GridSimulator.Api.Services;

public class DefaultRunSimulationService(IConfiguration configuration, IAgentFactory agentFactory,
    ILogger<DefaultRunSimulationService> logger) : IRunSimulationService
{
    public async Task<string> RunSimulationAsync(RunSimulationRequestModel request)
    {
        var demandCalcExecutor = new DemandCalculationExecutor(agentFactory.DemandCalculationAgent);
        var gridAnalysisExecutor = new GridAnalysisExecutor(agentFactory.GridAnalysisAgent);
        var actionPlanExecutor = new ActionPlanExecutor(agentFactory.ActionPlanAgent);

        var workflow = new WorkflowBuilder(demandCalcExecutor)
            //.AddEdge(demandCalcExecutor, gridAnalysisExecutor)
            //.AddEdge(gridAnalysisExecutor, actionPlanExecutor)
            .Build();

        StreamingRun run = await InProcessExecution.StreamAsync(workflow,
            input: new ChatMessage(ChatRole.User, Prompts.GetOutputReductionSimulationInput(request)));
        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
        await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
        {
            if (evt is WorkflowOutputEvent outputEvent)
            {
                Console.WriteLine($"{outputEvent}");
            }
        }

        return string.Empty;
    }
}

public interface IRunSimulationService
{
    Task<string> RunSimulationAsync(RunSimulationRequestModel request);
}