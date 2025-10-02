using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace GridSimulator.Api.Executors;

internal sealed class ConcurrentAggregationExecutor() : ReflectingExecutor<ConcurrentAggregationExecutor>("ConcurrentAggregationExecutor"),
    IMessageHandler<object>
{
    private readonly List<ChatMessage> _messages = [];

    public ValueTask HandleAsync(object message, IWorkflowContext context)
    {
        return ValueTask.CompletedTask;
    }
}