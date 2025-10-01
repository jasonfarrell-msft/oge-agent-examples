using GridSimulator.Api.Models;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace GridSimulator.Api.Executors;

internal sealed class GridAnalysisExecutor : ReflectingExecutor<GridAnalysisExecutor>,
    IMessageHandler<CalculatedDemandResult, string>
{
    private readonly int _currentOutput;
    private readonly int _maxOutput;
    private readonly int _rampRateMinutes;
    private readonly int _batteryCapacity;
    private readonly int _chargePercent;
    private readonly int _batteryDischargeRateMinutes;

    public GridAnalysisExecutor(AIAgent analysisAgent, RunSimulationRequestModel requestModel) : base("GridAnalysisExecutor")
    {
        _currentOutput = requestModel.BaselineGenerationParameters.CurrentOutput;
        _maxOutput = requestModel.BaselineGenerationParameters.MaxOutput;
        _rampRateMinutes = requestModel.BaselineGenerationParameters.RampRate;

        _batteryCapacity = requestModel.BaselineGenerationParameters.BatteryCapacity;
        _chargePercent = requestModel.BaselineGenerationParameters.ChargePercent;
        _batteryDischargeRateMinutes = requestModel.BaselineGenerationParameters.BatteryDischargeRate;
    }

    public ValueTask<string> HandleAsync(CalculatedDemandResult message, IWorkflowContext context)
    {
        throw new NotImplementedException();
    }
}
