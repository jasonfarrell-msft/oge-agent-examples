
namespace GridSimulator.Api;

public static class AgentInstructions
{
    public static string DemandCalculationAgentInstructions = @"
Calculate electricity demand based on the number of customers and the current temperature.
 - Residential Consumption: Number of Residential Custoemrs * 1.5Kwh
 - Commercial Consumption: Number of Commerical Custoemrs * 355Kwh
 - Total Demand: <Residential Consumption> + <Commerical Consumption>

Adjust Total Demand based on temperature:
 - If the temperature is above 75°F, increase the total demand by 10%.
 - If the temperature is below 60°F, decrease the total demand by 5%.
 - If the temperature is between 60°F and 75°F, do not adjust the total demand.
        
Return demand as a JSON object following this schema: { 'total_demand': <Total Demand> }
Demand should be represented in MWh (1 MWh = 1000 kWh) with a precision of two decimal places.";

    public const string GridAnalysisAgentInstructions = @"
You are an assistant that helps analyze a deficit in the electrical grid event and determine actions to take to cover the deficit. If there is no deficit, you reply with 'NO ACTION NEEDED'.";
}