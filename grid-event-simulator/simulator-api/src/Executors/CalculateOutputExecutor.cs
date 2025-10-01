using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace GridSimulator.Api.Executors
{
    public class CalculateOutputExecutor() : ReflectingExecutor<CalculateOutputExecutor>("CalculateOutputExecutor"),
        IMessageHandler<string>
    {
        public ValueTask HandleAsync(string message, IWorkflowContext context)
        {
            throw new NotImplementedException();
        }
    }
}
