using GridSimulator.Api.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace GridSimulator.Api.Services;

public class DefaultRunSimulationService(IConfiguration configuration, IAgentFactory agentFactory,
    ILogger<DefaultRunSimulationService> logger) : IRunSimulationService
{    
    public async Task<string> RunSimulationAsync(RunSimulationRequestModel request)
    {
        try
        {
#pragma warning disable SKEXP0110   
            var groupChat = new AgentGroupChat(
                agentFactory.DemandCalculationAgent,
                agentFactory.GridAnalysisAgent,
                agentFactory.ActionPlanAgent
            );
            var input = request.SimulationParameters.DemandIncreaseParameters is not null
                ? Prompts.GetDemandIncreaseSimulationInput()
                : Prompts.GetOutputReductionSimulationInput(request);

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
}

public interface IRunSimulationService
{
    Task<string> RunSimulationAsync(RunSimulationRequestModel request);
}