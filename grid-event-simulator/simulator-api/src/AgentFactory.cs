using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace GridSimulator.Api
{
    public class AgentFactory(IConfiguration configuration) : IAgentFactory
    {
        private Kernel? _sharedKernel;

        private Kernel CreateKernel()
        {
            var apiKey = configuration["API_KEY"] ?? throw new InvalidOperationException("API_KEY configuration is missing");
            return Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(
                    deploymentName: "gpt-5-mini-deployment",
                    endpoint: "https://orch-multi-agent-resource.cognitiveservices.azure.com",
                    apiKey: apiKey,
                    serviceId: "generative-service")
                .AddAzureOpenAIChatCompletion(
                    deploymentName: "o4-mini-deployment",
                    endpoint: "https://orch-multi-agent-resource.cognitiveservices.azure.com",
                    apiKey: apiKey,
                    serviceId: "reasoning-service")
                .Build();
        }

        private Kernel SharedKernel => _sharedKernel ??= CreateKernel();

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
