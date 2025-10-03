using GridSimulator.Api.Models;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GridSimulator.Api.Executors;

internal sealed class DemandCalculationExecutor(AIAgent demandCalcAgent) : ReflectingExecutor<DemandCalculationExecutor>("DemandCalculationExecutor"),
    IMessageHandler<RunSimulationRequestModel, RunSimulationRequestModel>
{
    public async ValueTask<RunSimulationRequestModel> HandleAsync(RunSimulationRequestModel simulationRequest, IWorkflowContext context)
    {
        string responseText = string.Empty;
        if (simulationRequest.SimulationType == SimulationType.OutputReduction)
        {
            var response = await demandCalcAgent.RunAsync($@"
Calculate the total demand using the following data:
 - Number of Residential Customers: {simulationRequest.DemandConfigurationParameters.ResidentialCustomers}
 - Number of Commercial Customers: {simulationRequest.DemandConfigurationParameters.CommercialCustomers}
 - Current Temperature: {simulationRequest.DemandConfigurationParameters.CurrentTemperature}");

            responseText = response.Text;
        }

        if (string.IsNullOrEmpty(responseText))
            throw new Exception("The response from the AI agent is empty.");

        var result = JsonSerializer.Deserialize<CalculatedDemandResult>(responseText) ?? throw new Exception("Failed to deserialize the AI response.");
        await context.QueueStateUpdateAsync<CalculatedDemandResult>(Constants.DemandCalcKey, result);
        
        return simulationRequest;
    }
}

public sealed class CalculatedDemandResult
{
    [JsonPropertyName("total_demand")] public decimal TotalDemand { get; set; }
}