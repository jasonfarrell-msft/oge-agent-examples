using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    public class TimeOfDayConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
                return DateTime.MinValue;

            if (DateTime.TryParseExact(value, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTime))
            {
                return DateTime.Today.Add(parsedTime.TimeOfDay);
            }

            throw new JsonException($"Unable to parse TimeOfDay '{value}'. Expected format: HH:mm");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("HH:mm"));
        }
    }

    public class SetCurrentWeatherRequest
    {
        public decimal WindSpeed { get; set; }
        public decimal Temperature { get; set; }
        
        [JsonConverter(typeof(TimeOfDayConverter))]
        public DateTime TimeOfDay { get; set; }
        
        public CloudCoverageEnum CloudCoverage { get; set; }
    }

    public class ReceiveWeather(ILogger<ReceiveWeather> logger, ICacheService cacheService)
    {
        [Function("receive_weather")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "receive_weather")] HttpRequest req)
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
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var typedReq = JsonSerializer.Deserialize<SetCurrentWeatherRequest>(requestBody, options);
                
                if (typedReq == null)
                {
                    return new BadRequestObjectResult("Failed to parse request body");
                }

                await cacheService.WriteAsync("CurrentWeather", typedReq);
                return new AcceptedResult();
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
