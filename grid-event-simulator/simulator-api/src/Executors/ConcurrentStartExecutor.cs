using GridSimulator.Api.Models;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace GridSimulator.Api.Executors;

internal sealed class ConcurrentStartExecutor() : ReflectingExecutor<ConcurrentStartExecutor>("ConcurrentStartExecutor")
    , IMessageHandler<RunSimulationRequestModel, CalculatedOutputResult>
{
    public async ValueTask<CalculatedOutputResult> HandleAsync(RunSimulationRequestModel request, IWorkflowContext context)
    {
        if (request.SimulationType == SimulationType.OutputReduction)
        {
            return new CalculatedOutputResult
            {
                TotalOutput = request.BaselineGenerationParameters.CurrentOutput * (request.SimulationParameters.OutputReductionParameters.ReduceOutputPercent / 100),
                MaxOutput = request.BaselineGenerationParameters.MaxOutput * (request.SimulationParameters.OutputReductionParameters.ReduceOutputPercent / 100)
            };
        }

        throw new Exception("Not implemented");
    }
}

public sealed class CalculatedOutputResult
{
    public int TotalOutput { get; set; }
    public int MaxOutput { get; set; }
}