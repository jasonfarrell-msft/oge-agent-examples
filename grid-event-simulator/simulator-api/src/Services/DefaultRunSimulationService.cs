using GridSimulator.Api.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

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

            var apiKey = configuration["API_KEY"] ?? throw new InvalidOperationException("API_KEY configuration is missing");
            var sharedKernel = Kernel.CreateBuilder()
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

            var demandAgent = new ChatCompletionAgent
            {
                Name = "DemandAgent",
                Instructions = Prompts.DemandCalculationAgentInstructions,
                Kernel = sharedKernel,
                Arguments = new KernelArguments(new OpenAIPromptExecutionSettings
                {
                    ServiceId = "generative-service"
                })
            };

#pragma warning disable SKEXP0110
            
            var groupChat = new AgentGroupChat(demandAgent);

            var simulationCase = request.SimulationParameters.DemandIncreaseParameters != null ? "Demand Increase" : "Output Reduction";
            var input = "Your goal is to create an action plan for handling the demand for electricity during a grid event.\n" +
                        "The grid event can be one of the following:\n" +
                        " - A spike in demand that will require an increase to the total electricity needed for a given duration\n" +
                        " - A reduction in output that will reduce the amount of electricity provided to the grid\n" +
                        $" For this simulation you will simulate a {simulationCase}\n\n" +

                        " The following baseline parameters are available:\n" +
                        $" - Current output: {request.BaselineGenerationParameters.CurrentOutput} MW\n" +
                        $" - Max output: {request.BaselineGenerationParameters.MaxOutput} MW\n" +
                        $" - Ramp rate (in minutes): {request.BaselineGenerationParameters.RampRate}/h\n" +
                        $" - Battery capacity: {request.BaselineGenerationParameters.BatteryCapacity} MW\n" +
                        $" - Battery Charge percent: {request.BaselineGenerationParameters.ChargePercent}%\n" +
                        $" - Battery Discharge rate (in MW): {request.BaselineGenerationParameters.BatteryDischargeRate} every 30 minutes/h\n" +
                        $" - Number of Residential customers: {request.DemandConfigurationParameters.ResidentialCustomers}\n" +
                        $" - Number of Commercial customers: {request.DemandConfigurationParameters.CommercialCustomers}\n" +
                        $" - Current temperature: {request.DemandConfigurationParameters.CurrentTemperature}°F" +
                        "\n\n" +
                        "In the case of Demand Increase, the following parameters are available:\n" +
                        $" - Peak Temperature: ${request.SimulationParameters.DemandIncreaseParameters.PeakTemperature}°F" +
                        $" - Time to Peak: ${request.SimulationParameters.DemandIncreaseParameters.TimeToPeak} minutes" +
                        $" - Peak Duration: ${request.SimulationParameters.DemandIncreaseParameters.PeakDuration} minutes" +
                        "\n\n" +
                        "In the case of Output Reduction, the following parameters are available:\n" +
                        $" - Reduce Output Percent: ${request.SimulationParameters.OutputReductionParameters.ReduceOutput}" +
                        "\n\n" +
                        "Respond with the following information\n" +
                        " - The final demand in MW\n" +
                        " - The final output in MW\n";

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