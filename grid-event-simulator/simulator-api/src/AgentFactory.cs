
using Azure.AI.Agents.Persistent;
using Azure.AI.OpenAI;
using GridSimulator.Api.Clients;
using GridSimulator.Api.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace GridSimulator.Api
{
    public class AgentFactory(IConfiguration configuration, IRatesApiHttpClient ratesApiHttpClient) : IAgentFactory
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
                Instructions = AgentInstructions.DemandCalculationAgentInstructions,
                Description = "Agent that calculates energy demand based on residential and commercial customer based on temperature"
            });

        public AIAgent GridAnalysisAgent => new ChatClientAgent(
            chatClient: GetChatClient("o4-mini-deployment"),
            options: new ChatClientAgentOptions
            {
                Name = "GridAnalysisAgent",
                Instructions = AgentInstructions.GridAnalysisAgentInstructions,
                Description = "Agent that analyzes the grid and provides recommendations to mitigate an energy shortage",
                ChatOptions = new ChatOptions
                {
                    Tools = [AIFunctionFactory.Create(GetRatesAsync)]
                }
            });

        public AIAgent ActionPlanAgent => new ChatClientAgent(
            chatClient: GetChatClient("gpt-5-mini-deployment"), new ChatClientAgentOptions
            {
                Name = "ActionPlanAgent",
                Instructions = AgentInstructions.ActionPlanAgentInstructions,
                Description = "Agent that creates an action plan based on the analysis of the grid",
            });

        // methods
        [Description("Get the current rate of electricity from the neighboring grid")]
        async Task<decimal?> GetRatesAsync()
        {
            var responseModel = await ratesApiHttpClient.GetRateAsync();
            if (responseModel is null)
                throw new Exception("Error getting rate");

            return responseModel.Rate;
        }
    }

    public interface IAgentFactory
    {
        AIAgent DemandCalculationAgent { get; }
        AIAgent GridAnalysisAgent { get; }
        AIAgent ActionPlanAgent { get; }
    }
}
