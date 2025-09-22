using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GridSimulator.Api.Models
{
    public class RunSimulationRequestModel
    {
        [JsonPropertyName("renewable_output")]
        public int RenewableOutputInMW { get; init; }

        [JsonPropertyName("traditional_output")]
        public int TraditionalOutputInMW { get; init; }

        [JsonPropertyName("traditional_ramp_rate")]
        public int TraditionalRampRateInMin { get; init; }
        
        [JsonPropertyName("battery_charge")]
        public int BatteryChargeInMW { get; init; }

        [JsonPropertyName("battery_discharge_rate")]
        public int BatteryDischargeInMW { get; init; }

        [JsonPropertyName("number_of_residential_customers")]
        public int NumberOfResidentialCustomers { get; init; }

        [JsonPropertyName("number_of_commercial_customers")]
        public int NumberOfCommercialCustomers { get; init; }
        
        [JsonPropertyName("parameters")]
        public required SimulationParameters Parameters { get; init; }
    }

    public class SimulationParameters
    {
        [JsonPropertyName("cloud_cover_increase_percentage")]
        public int CloudCoverIncreasePercentage { get; init; }

        [JsonPropertyName("temperature_increase_degrees")]
        public int TemperatureIncreaseDegrees { get; init; }

        [JsonPropertyName("wind_speed_decrease_percentage")]
        public int WindSpeedDecreasePercentage { get; init; }

        [JsonPropertyName("traditional_output_decrease_percentage")]
        public int TraditionalOutputDecreasePercentage { get; init; }

        [JsonPropertyName("renewable_output_decrease_percentage")]
        public int RenewableOutputDecreasePercentage { get; init; }
    }
}