using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Logging;

namespace Farrellsoft.Examples.Agents.MultiAgent.Functions
{
    public class NegotiateFunction(ILogger<NegotiateFunction  > logger)
    {
        [Function("negotiate")]
        public async Task<HttpResponseData> Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "negotiate")] HttpRequestData req)
        {
            var connectionString = Environment.GetEnvironmentVariable("SignalRConnectionString");

            var serviceManager = new ServiceManagerBuilder()
                .WithOptions(option => option.ConnectionString = connectionString)
                .BuildServiceManager();

            var hubContext = await serviceManager.CreateHubContextAsync("renewable_hub", default);
            var negotiateResponse = await hubContext.NegotiateAsync();

            var payload = JsonSerializer.Serialize(new
            {
                url = negotiateResponse.Url,
                accessToken = negotiateResponse.AccessToken,
                hubName = "renewable_hub"
            });

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(payload);
            response.Headers.Add("Content-Type", "application/json");

            return response;
        }
    }
}
