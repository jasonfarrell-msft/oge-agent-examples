using System.Text.Json.Serialization;

namespace GridSimulator.Api.Models;

public record RunSimulationRequestModel
{
    [JsonPropertyName("baseline")]
    public BaselineGenerationParameters BaselineGenerationParameters { get; init; }
    
    [JsonPropertyName("demand")]
    public DemandConfigurationParameters DemandConfigurationParameters { get; init; }
}

public record BaselineGenerationParameters
{
    [JsonPropertyName("current_output")]
    public int CurrentOutput { get; init; }
    
    [JsonPropertyName("max_output")]
    public int MaxOutput { get; init; }
    
    [JsonPropertyName("ramp_rate")]
    public int RampRate { get; init; }
    
    [JsonPropertyName("battery_capacity")]
    public int BatteryCapacity { get; init; }
    
    [JsonPropertyName("charge_percent")]
    public int ChargePercent { get; init; }
    
    [JsonPropertyName("discharge_rate")]
    public int BatteryDischargeRate { get; init; }
}

public record DemandConfigurationParameters
{
    [JsonPropertyName("residential_customers")]
    public int ResidentialCustomers { get; init; }
    
    [JsonPropertyName("commerical_customers")]
    public int CommercialCustomers { get; init; }
    
    [JsonPropertyName("current_temperature")]
    public int CurrentTemperature { get; init; }
}

public record SimulationParameters
{
    [JsonPropertyName("demand_increase")]
    public DemandIncreaseParameters? DemandIncreaseParameters { get; init; }
    
    [JsonPropertyName("output_reduction")]
    public OutputReductionParameters? OutputReductionParameters { get; init; }
}

public record DemandIncreaseParameters
{
    [JsonPropertyName("peak_temperature")]
    public int PeakTemperature { get; init; }
    
    [JsonPropertyName("time_to_peak")]
    public int TimeToPeak { get; init; }
    
    [JsonPropertyName("peak_duration")]
    public int PeakDuration { get; init; }
}

public record OutputReductionParameters
{
    [JsonPropertyName("reduce_output")]
    public bool ReduceOutput { get; init; }
}