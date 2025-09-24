using Azure.AI.OpenAI;
using GridSimulator.Api.Models;
using Microsoft.JSInterop;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Magentic;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace GridSimulator.Api.Services;

public class DefaultRunSimulationService(IConfiguration configuration, ILogger<DefaultRunSimulationService> logger) : IRunSimulationService
{
    public async Task<string> RunSimulationAsync(RunSimulationRequestModel request)
    {
        // create the demand agent
        var demandAgent = new ChatCompletionAgent
        {
            Name = "DemandAgent",
            Instructions = Prompts.DemandAgentInstructions,
            Kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(
                    deploymentName: "gpt-5-mini-deployment",
                    endpoint: "https://orch-multi-agent-resource.cognitiveservices.azure.com",
                    apiKey: configuration["API_KEY"] ?? throw new Exception("API_KEY is required")).Build()
        };

#pragma warning disable SKEXP0110
        var mainKernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: "gpt-5-mini-deployment",
                endpoint: "https://orch-multi-agent-resource.cognitiveservices.azure.com",
                apiKey: configuration["API_KEY"] ?? throw new Exception("API_KEY is required")).Build();
        var simulatorManager = new StandardMagenticManager(
            mainKernel.GetRequiredService<IChatCompletionService>(),
            new OpenAIPromptExecutionSettings());

        var orchestration = new MagenticOrchestration(
            manager: simulatorManager,
            members: new[] { demandAgent })
        {
            ResponseCallback = (response) =>
            {
                logger.LogInformation("Orchestration response: {Response}", response);
                return ValueTask.CompletedTask;
            }
        };

        var runtime = new InProcessRuntime();
        await runtime.StartAsync();
#pragma warning restore SKEXP0110

        return "Simulation completed successfully";
    }
}

public interface IRunSimulationService
{
    Task<string> RunSimulationAsync(RunSimulationRequestModel request);
}