using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace GridSimulator.Api.Executors;

internal sealed class DemandCalculationExecutor(AIAgent demandCalcAgent) : ReflectingExecutor<DemandCalculationExecutor>("DemandCalculationExecutor"),
    IMessageHandler<ChatMessage, CalculatedDemandResult>
{
    public async ValueTask<CalculatedDemandResult> HandleAsync(ChatMessage message, IWorkflowContext context)
    {
        var response  = await demandCalcAgent.RunAsync(message);
        return new CalculatedDemandResult();
    }
}

public sealed class CalculatedDemandResult
{

}