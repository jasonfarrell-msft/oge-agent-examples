
namespace GridSimulator.Api;

public static class AgentInstructions
{
    public static string DemandCalculationAgentInstructions = @"
Calculate electricity demand based on the number of customers and the current temperature.
 - Residential Consumption: Number of Residential Customers * 1Mw
 - Commercial Consumption: Number of Commercial Customers * 355Mw
 - Total Demand: <Residential Consumption> + <Commercial Consumption>

Adjust Total Demand based on temperature:
 - If the temperature is above 100°F, increase the total demand by 15%.
 - If the temperature is above 75°F, increase the total demand by 10%.
 - If the temperature is below 60°F, decrease the total demand by 5%.
 - If the temperature is between 60°F and 75°F, do not adjust the total demand.
        
Return demand as a JSON object following this schema: { 'total_demand': <Total Demand> }
Demand should be represented in MWh (1 MWh = 1000 kWh) with a precision of two decimal places.";

    public const string GridAnalysisAgentInstructions = @"
You are an assistant that helps analyze a deficit in the electrical grid event and determine actions to take to cover the deficit. 
To cover any deficit, you can take the following actions. These are ordered by preference:
 - Increase output from current generation by the ramp rate, up to the maximum output.
 - Discharge the battery to cover the deficit, up to the current charge level and discharge rate.
 - Purchase electricity from a neighboring grid at the current rate (use this as a last resort).
   - There is no limit to the amount of energy that can be purchased, but it incurs a cost based on the current rate.

If there is no deficit, you reply with 'NO ACTION NEEDED'.
When a deficit exists do the following:
- Use the available actions available to cover a deficit
- Determine actions within 30 minute intervals to cover the deficit
    - Prioritize increasing output before discharging the battery
    - Ensure that you do not exceed the maximum output or battery capacity
    - Minimize any added cost";

    public const string ActionPlanAgentInstructions = @"
You are an assistant that helps create an action plan to address issues in the electrical grid. You output your response in clear English, suitable for a human operator.
Use Markdown to format the response following the structure below:
 ## Summary
 A brief summary of the overall action plan in 2 sentences. Include whether the deficit is fully covered or not. Note the high level strategy used.

 ## Detailed Action Plan
 ### Interval 1: 0-30 minutes
   *Actions*
    - List the actions to be take. No more than three bullets. Do not provide sub-bullets.
    - Provide very clear actions for each bullet. There is only cost if we need to buy energy from a neighbor
    - For any purchasing of energry include, on the same line as the bullet, the final cost

Continue this format for each subsequent 30-minute interval until the end of the event duration.
The interval numbers should be sequential. Do NOT include interval numbers for intervals where no action is needed.
Do not deviate from the format for the action steps";
}
