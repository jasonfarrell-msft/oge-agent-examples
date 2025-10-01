using GridSimulator.Api.Models;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace GridSimulator.Api.Executors;

internal sealed class ConcurrentStartExecutor() : ReflectingExecutor<ConcurrentStartExecutor>("ConcurrentStartExecutor")
    , IMessageHandler<RunSimulationRequestModel>
{
    public async ValueTask HandleAsync(RunSimulationRequestModel request, IWorkflowContext context)
    {
        if (request.SimulationType == SimulationType.OutputReduction)
            await context.SendMessageAsync(new ChatMessage(ChatRole.User, Prompts.GetOutputReductionSimulationInput(request)));
    }
}
