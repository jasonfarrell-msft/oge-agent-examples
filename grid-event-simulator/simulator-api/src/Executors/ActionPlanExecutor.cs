using Microsoft.Agents.AI;
using Microsoft.Agents.Workflows;
using Microsoft.Agents.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace GridSimulator.Api.Executors;

internal sealed class ActionPlanExecutor(AIAgent actionPlanAgent) : ReflectingExecutor<ActionPlanExecutor>("ActionPlanExecutor"),
    IMessageHandler<ChatMessage, string>
{
    public ValueTask<string> HandleAsync(ChatMessage message, IWorkflowContext context)
    {
        throw new NotImplementedException();
    }
}
