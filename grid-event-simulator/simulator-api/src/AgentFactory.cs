
using Azure.AI.Agents.Persistent;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace GridSimulator.Api
{
    public class AgentFactory(IConfiguration configuration) : IAgentFactory
    {
        IChatClient GetChatClient(string deploymentName) => new AzureOpenAIClient(
                endpoint: new Uri("https://orch-multi-agent-resource.cognitiveservices.azure.com"),
                credential: new System.ClientModel.ApiKeyCredential(configuration["API_KEY"] ?? throw new InvalidOperationException("API_KEY configuration is missing")),
                options: new AzureOpenAIClientOptions())
            .GetChatClient(deploymentName)
            .AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();

        public AIAgent DemandCalculationAgent => new ChatClientAgent(
            chatClient: GetChatClient("gpt-5-mini-deployment"), new ChatClientAgentOptions
            {
                Name = "DemandCalculationAgent",
                Instructions = Prompts.DemandCalculationAgentInstructions,
                Description = "Agent that calculates energy demand based on residential and commerical customer base and a temperature",
            });

        public AIAgent GridAnalysisAgent => new ChatClientAgent(
            chatClient: GetChatClient("o4-mini-deployment"), new ChatClientAgentOptions
            {
                Name = "GridAnalysisAgent",
                Instructions = Prompts.GridAnalysisAgentInstructions,
                Description = "Agent that analyzes the grid and provides recommendations to mitigate an energy shortage",
            });

        public AIAgent ActionPlanAgent => new ChatClientAgent(
            chatClient: GetChatClient("gpt-5-mini-deployment"), new ChatClientAgentOptions
            {
                Name = "ActionPlanAgent",
                Instructions = Prompts.ActionPlanAgentInstructions,
                Description = "Agent that creates an action plan based on the analysis of the grid",
            });

        public AIAgent[] AllAgents => new[]
        {
            DemandCalculationAgent,
            GridAnalysisAgent,
            ActionPlanAgent
        };
    }

    public interface IAgentFactory
    {
        AIAgent DemandCalculationAgent { get; }
        AIAgent GridAnalysisAgent { get; }
        AIAgent ActionPlanAgent { get; }

        AIAgent[] AllAgents { get; }
    }
}
