using GridSimulator.Api.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace GridSimulator.Api.Services;

public class DefaultRunSimulationService(IConfiguration configuration, ILogger<DefaultRunSimulationService> logger) : IRunSimulationService
{
    private ChatHistory chatHistory = [];
    
    public async Task<string> RunSimulationAsync(RunSimulationRequestModel request)
    {
        try
        {
            logger.LogInformation("Starting simulation with {ResidentialCustomers} residential and {CommercialCustomers} commercial customers at {Temperature}°F", 
                request.DemandConfigurationParameters.ResidentialCustomers,
                request.DemandConfigurationParameters.CommercialCustomers,
                request.DemandConfigurationParameters.CurrentTemperature);

            var apiKey = configuration["API_KEY"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("API_KEY configuration is missing");
            }

            var sharedKernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(
                    deploymentName: "gpt-5-mini-deployment",
                    endpoint: "https://orch-multi-agent-resource.cognitiveservices.azure.com",
                    apiKey: apiKey)
                .Build();

            var demandAgent = new ChatCompletionAgent
            {
                Name = "DemandAgent",
                Instructions = Prompts.DemandAgentInstructions,
                Kernel = sharedKernel
            };

#pragma warning disable SKEXP0110
            
            var groupChat = new AgentGroupChat(demandAgent);

            var input = $@"Calculate the total electricity demand using these parameters:
- Residential customers: {request.DemandConfigurationParameters.ResidentialCustomers}
- Commercial customers: {request.DemandConfigurationParameters.CommercialCustomers}  
- Current temperature: {request.DemandConfigurationParameters.CurrentTemperature}°F

Provide only the final result in MW with two decimal places.";

            groupChat.AddChatMessage(new ChatMessageContent(AuthorRole.User, input));

            var responses = new List<ChatMessageContent>();
            await foreach (var response in groupChat.InvokeAsync())
            {
                responses.Add(response);
                
                if (response.Role == AuthorRole.Assistant)
                {
                    break;
                }
            }

            var finalResponse = responses.LastOrDefault()?.Content ?? "No response received";
            
            return finalResponse;

            
#pragma warning restore SKEXP0110
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during simulation");
            return $"Error: {ex.Message}";
        }
    }
    
    ValueTask responseCallback(ChatMessageContent response)
    {
        try 
        {
            chatHistory.Add(response);
            var contentLength = response.Content?.Length ?? 0;
            var truncatedContent = contentLength > 0 ? response.Content?.Substring(0, Math.Min(200, contentLength)) : "Empty";
            logger.LogInformation("Agent Response - {Role} ({Author}): {Content}", 
                response.Role, 
                response.AuthorName ?? "Unknown", 
                truncatedContent);
            return ValueTask.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in response callback");
            return ValueTask.CompletedTask;
        }
    }
}

public interface IRunSimulationService
{
    Task<string> RunSimulationAsync(RunSimulationRequestModel request);
}