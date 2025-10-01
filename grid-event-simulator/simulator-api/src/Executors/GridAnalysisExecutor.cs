using GridSimulator.Api.Models;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace GridSimulator.Api.Executors;

internal sealed class GridAnalysisExecutor(AIAgent analysisAgent) : ReflectingExecutor<GridAnalysisExecutor>("GridAnalysisAgentExecutor"),
    IMessageHandler<CalculatedDemandResult, string>
{
    public ValueTask<string> HandleAsync(CalculatedDemandResult message, IWorkflowContext context)
    {
        return ValueTask.FromResult("NO ACTION NEEDED");
    }
}
