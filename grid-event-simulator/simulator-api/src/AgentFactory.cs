using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace GridSimulator.Api
{
    public class AgentFactory(IKernelFactory kernelFactory) : IAgentFactory
    {
        private Kernel? _sharedKernel;

        private Kernel SharedKernel => _sharedKernel ??= kernelFactory.GetKernel();

        public ChatCompletionAgent DemandCalculationAgent => new ChatCompletionAgent
        {
            Name = "DemandACalculationAgent",
            Instructions = Prompts.DemandCalculationAgentInstructions,
            Kernel = SharedKernel,
            Arguments = new KernelArguments(new OpenAIPromptExecutionSettings
            {
                ServiceId = "generative-service"
            })
        };

        public ChatCompletionAgent GridAnalysisAgent => new ChatCompletionAgent
        {
            Name = "GridAnalysisAgent",
            Instructions = Prompts.GridAnalysisAgentInstructions,
            Kernel = SharedKernel,
            Arguments = new KernelArguments(new OpenAIPromptExecutionSettings
            {
                ServiceId = "reasoning-service"
            })
        };

        public ChatCompletionAgent ActionPlanAgent => new ChatCompletionAgent
        {
            Name = "ActionPlanAgent",
            Instructions = Prompts.ActionPlanAgentInstructions,
            Arguments = new KernelArguments(new OpenAIPromptExecutionSettings
            {
                ServiceId = "generative-service"
            }),
        };
    }

    public interface IAgentFactory
    {
        ChatCompletionAgent DemandCalculationAgent { get; }
        ChatCompletionAgent GridAnalysisAgent { get; }
        ChatCompletionAgent ActionPlanAgent { get; }
    }
}
