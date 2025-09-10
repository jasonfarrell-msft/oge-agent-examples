using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Farrellsoft.Examples.Agents.MultiAgent.Functions
{
    public class SetDemandRequest
    {
        public int NumberOfCustomers { get; set; }
        public decimal Temperature { get; set; }
    }

    public class ReceiveDemandFunction(ILogger<ReceiveDemandFunction> logger, ICacheService cacheService)
    {
        [Function("receive_demand")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "receive_demand")] HttpRequestData req)
        {
            logger.LogInformation("Receive demand function processed a request.");

            try
            {
                // Read the request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync("Request body is empty");
                    return badRequestResponse;
                }

                // Configure JSON serializer options for case-insensitive deserialization
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                // Deserialize the request body
                var demandRequest = JsonSerializer.Deserialize<SetDemandRequest>(requestBody, options);
                
                if (demandRequest == null)
                {
                    var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync("Failed to parse request body");
                    return badRequestResponse;
                }

                // Write to Redis cache with key "currentDemand"
                await cacheService.WriteAsync("currentDemand", demandRequest);

                // Return 204 Accepted result
                return req.CreateResponse(HttpStatusCode.Accepted);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Invalid JSON in request body");
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync($"Invalid JSON: {ex.Message}");
                return badRequestResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing demand request");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("Internal server error");
                return errorResponse;
            }
        }
    }
}
