
using System.Net;
using System.Text;
using Farrellsoft.Examples.Agents.SingleAgent.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Farrellsoft.Examples.Agents.SingleAgent;

public class UploadFunction(ILogger<UploadFunction> logger, IAgentFileService agentFileService)
{
    [Function("Upload")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload")] HttpRequestData req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");

        try
        {
            // Read the request body
            using var reader = new MemoryStream();
            await req.Body.CopyToAsync(reader);
            var body = reader.ToArray();

            if (body == null || body.Length == 0)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("No file data received");
                return badResponse;
            }

            // For now, we'll treat the entire body as the file content
            // In a real implementation, you'd parse the multipart form data
            var fileName = req.Headers.TryGetValues("X-File-Name", out var fileNameValues)
                ? fileNameValues.FirstOrDefault() ?? "uploaded-file.pdf"
                : "uploaded-file.pdf";

            var data = BinaryData.FromBytes(body);
            logger.LogInformation("Processing file upload: {FileName}", fileName);
            await agentFileService.ProcessFileAsync(data, fileName);

            var response = req.CreateResponse(HttpStatusCode.Accepted);
            await response.WriteStringAsync("File uploaded successfully");
            return response;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing file upload");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("Error processing file upload");
            return errorResponse;
        }
    }
}