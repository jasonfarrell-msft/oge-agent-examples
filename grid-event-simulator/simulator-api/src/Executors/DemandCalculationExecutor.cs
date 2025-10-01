using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GridSimulator.Api.Executors;

internal sealed class DemandCalculationExecutor(AIAgent demandCalcAgent,
    int numberOfResidents,
    int numberOfCommercials,
    int currentTemperature) : ReflectingExecutor<DemandCalculationExecutor>("DemandCalculationExecutor"),
    IMessageHandler<ChatMessage, CalculatedDemandResult>
{
    public async ValueTask<CalculatedDemandResult> HandleAsync(ChatMessage message, IWorkflowContext context)
    {
        try
        {
            var response = await demandCalcAgent.RunAsync($@"
Calculate the total demand using the following data:
 - Number of Residential Customers: {numberOfResidents}
 - Number of Commercial Customers: {numberOfCommercials}
 - Current Temperature: {currentTemperature}");

            var responseText = response.Text;
            if (string.IsNullOrEmpty(responseText))
                throw new Exception("The response from the AI agent is empty.");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            return JsonSerializer.Deserialize<CalculatedDemandResult>(response.Text, options) ?? throw new Exception("Failed to deserialize the AI response.");
        }
        catch
        {
            throw;
        }
    }
}

public sealed class CalculatedDemandResult
{
    [JsonPropertyName("total_demand")]
    public decimal TotalDemand { get; set; }
}