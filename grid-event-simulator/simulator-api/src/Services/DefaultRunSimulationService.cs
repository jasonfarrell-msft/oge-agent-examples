using Azure;
using GridSimulator.Api.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace GridSimulator.Api.Services;

public class DefaultRunSimulationService(IConfiguration configuration, ILogger<DefaultRunSimulationService> logger) : IRunSimulationService
{
    public Task<string> RunSimulationAsync(RunSimulationRequestModel request)
    {
        // create the demand agent
        var builder = Kernel.CreateBuilder();
        builder.AddAzureOpenAIChatCompletion(
            deploymentName: "gpt-5-mini-deployment",
            endpoint: "https://orch-multi-agent-resource.cognitiveservices.azure.com",
            apiKey: string.Empty);
        
        var kernel = builder.Build();
        var demandAgent = new ChatCompletionAgent
        {
            Name = "DemandAgent",
            Instructions = "Filler Instructions",
            Kernel = kernel
        };
        
        return Task.FromResult(string.Empty);
    }
}

public interface IRunSimulationService
{
    Task<string> RunSimulationAsync(RunSimulationRequestModel request);
}