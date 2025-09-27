using System.Text.Json.Serialization;

namespace GridSimulator.Api.Models;

public record GetRateResponseModel(
    [property: JsonPropertyName("rate")] decimal Rate,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("unit")] string Unit)
{
}