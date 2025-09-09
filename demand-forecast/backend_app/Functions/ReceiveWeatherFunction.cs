using System;
using System.IO;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Farrellsoft.Examples.Agents.MultiAgent.Functions
{
    public enum CloudCoverageEnum
    {
        Clear = 0,
        PartlyCloudy = 1,
        MostlyCloudy = 2,
        Cloudy = 3
    }

    public class SetCurrentWeatherRequest
    {
        public decimal WindSpeed { get; set; }
        public decimal Temperature { get; set; }
        public DateTime TimeOfDay { get; set; }
        public CloudCoverageEnum CloudCoverage { get; set; }
    }

    public class ReceiveWeather(ILogger<ReceiveWeather> logger)
    {
        [Function("receive_weather")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "receive_weather")] HttpRequest req)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody;
            using (var reader = new StreamReader(req.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return new BadRequestObjectResult("Request body is empty");
            }

            try
            {
                using var doc = JsonDocument.Parse(requestBody);
                var root = doc.RootElement;

                decimal windSpeed = 0m;
                if (root.TryGetProperty("WindSpeed", out var wsEl) && wsEl.ValueKind == JsonValueKind.Number)
                {
                    windSpeed = wsEl.GetDecimal();
                }

                decimal temperature = 0m;
                if (root.TryGetProperty("Temperature", out var tEl) && tEl.ValueKind == JsonValueKind.Number)
                {
                    temperature = tEl.GetDecimal();
                }

                DateTime timeOfDay = DateTime.MinValue;
                if (root.TryGetProperty("TimeOfDay", out var timeEl) && timeEl.ValueKind == JsonValueKind.String)
                {
                    var timeStr = timeEl.GetString();
                    if (!string.IsNullOrEmpty(timeStr))
                    {
                        if (DateTime.TryParseExact(timeStr, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTime))
                        {
                            timeOfDay = DateTime.Today.Add(parsedTime.TimeOfDay);
                        }
                        else
                        {
                            return new BadRequestObjectResult("TimeOfDay must be in HH:mm 24-hour format");
                        }
                    }
                }

                int cloudCoverageInt = 0;
                if (root.TryGetProperty("CloudCoverage", out var ccEl) && ccEl.ValueKind == JsonValueKind.Number)
                {
                    cloudCoverageInt = ccEl.GetInt32();
                }

                CloudCoverageEnum cloudCoverage = Enum.IsDefined(typeof(CloudCoverageEnum), cloudCoverageInt)
                    ? (CloudCoverageEnum)cloudCoverageInt
                    : CloudCoverageEnum.Clear;

                var typedReq = new SetCurrentWeatherRequest
                {
                    WindSpeed = windSpeed,
                    Temperature = temperature,
                    TimeOfDay = timeOfDay,
                    CloudCoverage = cloudCoverage
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var responseJson = JsonSerializer.Serialize(typedReq, options);

                return new ContentResult
                {
                    Content = responseJson,
                    ContentType = "application/json",
                    StatusCode = 200
                };
            }
            catch (JsonException je)
            {
                return new BadRequestObjectResult("Invalid JSON: " + je.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error parsing weather request");
                return new ObjectResult("Internal server error") { StatusCode = 500 };
            }
        }
    }
}
