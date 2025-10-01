

using GridSimulator.Api.Models;

namespace GridSimulator.Api;
public static class Prompts
{
    

    public const string ActionPlanAgentInstructions = @"
You are an assistant that helps create a detailed action plan based on the analysis of an electrical grid deficit event. You use clear English and provide specific steps to be taken by a human operator.
You are provided with the analysis of the grid situation, including the identified deficit and the recommended actions to cover it.
Your steps should be clear, actionable, and easy to follow.

Output the following:
A summary of the action in a 2 sentance paragraph. Detailing the high level action to cover the deficit.
Example: Using primarily battery in the short term to support a ramp up of power generation the deficit can be covered for the duration of the event. This approach will have neglible costs to grid operation.

Provide the detailed action plan for each 30 minute interval where action is required
Example:
```
- T = 0 minutes (immediate)
  - Actions:
    - Dispatch energy storage: +100 MW immediate.
    - Command other online units to start ramping up at +100 MW per 15 minutes.
  - Supplied toward deficit in the first 15 minutes: storage 100 MW + generators +100 MW = 200 MW (remaining deficit 400 MW).
  - Status after 30 minutes:
    - Generators added = 200 MW (two ×15-min steps), storage = 100 MW → total = 300 MW
    - Remaining deficit = 600 − 300 = 300 MW

- T = 30 minutes
  - Actions (continue):
    - Continue generator ramp (+100 MW per 15-min step).
    - Keep storage discharging at 100 MW.
  - Status after 60 minutes:
    - Generators added = 400 MW, storage = 100 MW → total = 500 MW
    - Remaining deficit = 600 − 500 = 100 MW

- T = 60 minutes
  - Actions (continue):
    - Continue generator ramp. Only 100 MW more of generation is actually required to close the gap.
    - Keep storage discharging at 100 MW.
  - Important intermediate: because generators ramp in 15-minute increments, at T = 75 minutes the next +100 MW generator increment becomes available.
  - Status at T = 75 minutes:
    - Generators added = 500 MW, storage = 100 MW → total = 600 MW
    - Remaining deficit = 0 MW (shortfall fully covered at T = 75 minutes)
```

Provide this output in a Markdown format suitable for display in a web application. Do not include any other sections or calculation summaries";

    public static string? GetDemandIncreaseSimulationInput()
    {
        return string.Empty;
    }

    public static string? GetOutputReductionSimulationInput(RunSimulationRequestModel requestModel)
    {
        return "Your goal is to create a cost effective and timely action plan to cover a electrical grid deficit due to a sudden drop in power generation for a period of time.\n" +
            "To achieve this you will need to determine the demand on the grid using the following parameters:\n" +
            $" - Number of Residential Customers: {requestModel.DemandConfigurationParameters.ResidentialCustomers}\n" +
            $" - Number of Commercial Customers: {requestModel.DemandConfigurationParameters.CommercialCustomers}\n" +
            $" - Current Temperature: {requestModel.DemandConfigurationParameters.CurrentTemperature}°F\n\n" +

            "To determine the power available within the grid to the following:" +
            $" - Reduce {requestModel.BaselineGenerationParameters.CurrentOutput} MW by {requestModel.SimulationParameters.OutputReductionParameters.ReduceOutputPercent}% for {requestModel.SimulationParameters.OutputReductionParameters.ReductionDurationMinutes} minutes\n\n" +

            "If there is a deficit, review the following strategies to cover the deficit, including:\n" +
            $" - Increase the output from other power sources to the maximum available considering a ramp rate of 100 MW per {requestModel.BaselineGenerationParameters.RampRate} minutes.\n" +
            $" - Leverage energy storage systems to provide additional power, considering:\n" +
            $"   - Current Storage Capacity: {requestModel.BaselineGenerationParameters.ChargePercent}% of {requestModel.BaselineGenerationParameters.BatteryCapacity} MW\n" +
            $"   - Maximum Discharge Rate: {requestModel.BaselineGenerationParameters.BatteryDischargeRate} MW per 15 minutes\n" +
            $" - Purchase additional energy from neighboring grids based on available rates\n" +
            "As part of analysis, provider steps that should be taken, if any, every 30 minutes until the deficit is gone. You may use multiple strategies to cover the deficit.\n\n" +

            "Based on the strategy or strategies chosen create an action plan for review by a human operator";
    }
}
