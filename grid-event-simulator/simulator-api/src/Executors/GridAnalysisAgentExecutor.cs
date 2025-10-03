using GridSimulator.Api.Models;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace GridSimulator.Api.Executors;

public class GridAnalysisAgentExecutor(AIAgent gridAnalysisAgent) : ReflectingExecutor<GridAnalysisAgentExecutor>("GridAnalysisAgentExecutor"),
    IMessageHandler<RunSimulationRequestModel, string>
{
    public ValueTask<string> HandleAsync(RunSimulationRequestModel message, IWorkflowContext context)
    {
        throw new NotImplementedException();
    }
}