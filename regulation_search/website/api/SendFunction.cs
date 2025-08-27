using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Farrellsoft.Examples.Agents.SingleAgent.Models;
using Farrellsoft.Examples.Agents.SingleAgent.Services;

namespace Farrellsoft.Examples.Agents.SingleAgent;

public class SendFunction
{
    private readonly ILogger _logger;
    private readonly IProcessQueryService _processQueryService;

    public SendFunction(ILoggerFactory loggerFactory, IProcessQueryService processQueryService)
    {
        _logger = loggerFactory.CreateLogger<SendFunction>();
        _processQueryService = processQueryService;
    }

    [Function("Send")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "send")] HttpRequestData req)
    {
        _logger.LogInformation("Processing request to /api/send");

        try
        {
            // Read and parse the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            if (string.IsNullOrWhiteSpace(requestBody))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Request body is required");
                return badRequestResponse;
            }

            QueryRequest? request;
            try
            {
                request = JsonSerializer.Deserialize<QueryRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid JSON format");
                return badRequestResponse;
            }

            // Validate that query field is present and not empty
            if (request?.Query == null || string.IsNullOrWhiteSpace(request.Query))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Field 'query' is required and cannot be empty");
                return badRequestResponse;
            }

            // Process the query using the service
            var result = await _processQueryService.ProcessQuery(request.Query, request.ThreadId);

            // Create successful response
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            
            var jsonResponse = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            await response.WriteStringAsync(jsonResponse);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request");
            
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("An internal server error occurred");
            return errorResponse;
        }
    }
}
