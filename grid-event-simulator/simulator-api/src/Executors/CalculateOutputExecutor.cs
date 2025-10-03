using GridSimulator.Api.Models;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace GridSimulator.Api.Executors;

public class CalculateOutputExecutor() : ReflectingExecutor<CalculateOutputExecutor>("CalculateOutputExecutor"),
    IMessageHandler<RunSimulationRequestModel, RunSimulationRequestModel>
{
    public async ValueTask<RunSimulationRequestModel> HandleAsync(RunSimulationRequestModel requestModel, IWorkflowContext context)
    {
        var outputCalcResult = new CalculatedOutputResult(
            CurrentPowerOutput: requestModel.BaselineGenerationParameters.CurrentOutput * ((decimal)requestModel.SimulationParameters.OutputReductionParameters.ReduceOutputPercent / 100),
            MaxPowerOutput: requestModel.BaselineGenerationParameters.MaxOutput * ((decimal)requestModel.SimulationParameters.OutputReductionParameters.ReduceOutputPercent / 100)
        );
        
        await context.QueueStateUpdateAsync(Constants.OutputCalcKey, outputCalcResult, scopeName: "my-scope");
        return requestModel;
    }
}

public record CalculatedOutputResult(decimal CurrentPowerOutput, decimal MaxPowerOutput)
{
}