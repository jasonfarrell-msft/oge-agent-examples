using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace GridSimulator.Api.Executors;

internal sealed class ConcurrentAggregationExecutor() : ReflectingExecutor<ConcurrentAggregationExecutor>("ConcurrentAggregationExecutor"),
    IMessageHandler<ChatMessage>
{
    private readonly List<ChatMessage> _messages = [];

    public async ValueTask HandleAsync(ChatMessage message, IWorkflowContext context)
    {
        this._messages.Add(message);

        if (this._messages.Count == 2)
        {
            var formattedMessages = string.Join(Environment.NewLine,
                this._messages.Select(m => $"{m.AuthorName}: {m.Text}"));
            await context.YieldOutputAsync(formattedMessages);
        }
    }
}