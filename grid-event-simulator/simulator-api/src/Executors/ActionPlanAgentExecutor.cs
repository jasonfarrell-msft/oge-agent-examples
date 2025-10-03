using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace GridSimulator.Api.Executors;

public class ActionPlanAgentExecutor(AIAgent actionPlanAgent) : ReflectingExecutor<ActionPlanAgentExecutor>("ActionPlanAgentExecutor"),
    IMessageHandler<string, string>
{
    public async ValueTask<string> HandleAsync(string analysisText, IWorkflowContext context)
    {
        var response = await actionPlanAgent.RunAsync($@"
Create a detailed action plan based on the following grid analysis:
{analysisText}");

       if (string.IsNullOrEmpty(response.Text))
            throw new Exception("The response from the AI agent is empty.");
        return response.Text;
    }
}
