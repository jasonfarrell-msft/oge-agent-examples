using System.ComponentModel;
using GridSimulator.Api.Clients;

namespace GridSimulator.Api.Plugins;

/*public class RatesPlugin(IRatesApiHttpClient ratesHttpClient)
{
    [KernelFunction, Description("Get the current rate of electricity from the neighboring grid")]
    public async Task<decimal?> GetRatesAsync()
    {
        var responseModel = await ratesHttpClient.GetRateAsync();
        if (responseModel is null)
            throw new Exception("Error getting rate");
        
        return responseModel.Rate;
    }
}*/