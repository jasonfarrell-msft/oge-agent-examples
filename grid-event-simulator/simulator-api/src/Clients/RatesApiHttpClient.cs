using System.Text.Json;
using GridSimulator.Api.Models;

namespace GridSimulator.Api.Clients;

public class RatesApiHttpClient(HttpClient httpClient) : IRatesApiHttpClient
{
    public async Task<GetRateResponseModel?> GetRateAsync()
    {
        var response = await httpClient.GetAsync($"https://func-rate-api-eus2-mx01.azurewebsites.net/api/get_rate");
        response.EnsureSuccessStatusCode();
            
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GetRateResponseModel>(content);
    }
}

public interface IRatesApiHttpClient
{
    Task<GetRateResponseModel?> GetRateAsync();
}