
using Azure.AI.Agents.Persistent;
using Azure.AI.OpenAI;
using GridSimulator.Api.Models;
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

        public AIAgent GetDemandCalculationAgent(RunSimulationRequestModel request) => new ChatClientAgent(
            chatClient: GetChatClient("gpt-5-mini-deployment"), new ChatClientAgentOptions
            {
                Name = "DemandCalculationAgent",
                Instructions = AgentInstructions.DemandCalculationAgentInstructions,
                Description = "Agent that calculates energy demand based on residential and commercial customer based on temperature",
            });

        public AIAgent GridAnalysisAgent => new ChatClientAgent(
            chatClient: GetChatClient("o4-mini-deployment"), new ChatClientAgentOptions
            {
                Name = "GridAnalysisAgent",
                Instructions = AgentInstructions.GridAnalysisAgentInstructions,
                Description = "Agent that analyzes the grid and provides recommendations to mitigate an energy shortage",
            });

        public AIAgent ActionPlanAgent => new ChatClientAgent(
            chatClient: GetChatClient("gpt-5-mini-deployment"), new ChatClientAgentOptions
            {
                Name = "ActionPlanAgent",
                Instructions = Prompts.ActionPlanAgentInstructions,
                Description = "Agent that creates an action plan based on the analysis of the grid",
            });
    }

    public interface IAgentFactory
    {
        AIAgent GetDemandCalculationAgent(RunSimulationRequestModel request);
        AIAgent GridAnalysisAgent { get; }
        AIAgent ActionPlanAgent { get; }
    }
}
