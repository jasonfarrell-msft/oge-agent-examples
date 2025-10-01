using GridSimulator.Api.Models;

namespace GridSimulator.Api;

public static class AgentInstructions
{
    public static string GetDemandCalculationAgentInstructions(RunSimulationRequestModel requestModel)
    {
        return $@"
Calculate electricity demand based on the number of customers and the current temperature.
Residential Consumption: {requestModel.DemandConfigurationParameters.ResidentialCustomers} * 1.5Kwh
Commercial Consumption: {requestModel.DemandConfigurationParameters.CommercialCustomers} * 355Kwh
Total Demand: <Residential Consumption> + <Commerical Consumption>

Adjust Total Demand based on temperature:
 - If the temperature is above 75°F, increase the total demand by 10%.
 - If the temperature is below 60°F, decrease the total demand by 5%.
 - If the temperature is between 60°F and 75°F, do not adjust the total demand.\n\n" +

"Return demand as a JSON object following this schema: { 'total_demand': <Total Demand> }\n" +
"Demand should be represented in MWh (1 MWh = 1000 kWh) with a precision of two decimal places.";
    }
}