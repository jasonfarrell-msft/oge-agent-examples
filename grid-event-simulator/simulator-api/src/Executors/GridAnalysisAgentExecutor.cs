using GridSimulator.Api.Models;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace GridSimulator.Api.Executors;

public class GridAnalysisAgentExecutor(AIAgent gridAnalysisAgent) : ReflectingExecutor<GridAnalysisAgentExecutor>("GridAnalysisAgentExecutor"),
    IMessageHandler<RunSimulationRequestModel, string>
{
    public async ValueTask<string> HandleAsync(RunSimulationRequestModel requestModel, IWorkflowContext context)
    {
        var demandCalcResult = await context.ReadStateAsync<CalculatedDemandResult>(Constants.DemandCalcKey, scopeName: "my-scope");
        if (demandCalcResult is null)
            throw new Exception("Demand calculation result is missing.");

        var outputCalcResult = await context.ReadStateAsync<CalculatedOutputResult>(Constants.OutputCalcKey, scopeName: "my-scope");
        if (outputCalcResult is null)
            throw new Exception("Output calculation result is missing.");

        var availableBatteryPower = requestModel.BaselineGenerationParameters.BatteryCapacity * ((decimal)requestModel.BaselineGenerationParameters.ChargePercent / 100);
        var agentPrompt = $@"
Determine an action plan given the following data:
- Current Power Output: {outputCalcResult.CurrentPowerOutput} MW
- Current Max Output: {outputCalcResult.MaxPowerOutput} MW
- Grid Ramp Rate: {requestModel.BaselineGenerationParameters.RampRate} minutes per 100 MW
- Current Demand: {demandCalcResult.TotalDemand} MW
- Available Battery Power: {availableBatteryPower} MW
- Battery Discharge Rate: {requestModel.BaselineGenerationParameters.BatteryDischargeRate} MW per 30 minutes
";

        if (requestModel.SimulationType == SimulationType.OutputReduction)
        {
            agentPrompt += $@"Here are the parameters of the output reduction event:
- Duration: {requestModel.SimulationParameters.OutputReductionParameters.ReductionDurationMinutes} minutes";
        }

        var response = await gridAnalysisAgent.RunAsync(agentPrompt);
        if (string.IsNullOrEmpty(response.Text))
            throw new Exception("The response from the AI agent is empty.");

        return response.Text;
    }
}