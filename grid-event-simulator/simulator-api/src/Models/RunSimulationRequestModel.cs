using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridSimulator.Api.Models
{
    public class RunSimulationRequestModel
    {
        public int RenewableOutputInMW { get; init; }
        public int TraditionalOutputInMW { get; init; }
        public int TraditionalRampRateInMin { get; init; }

        public int BatteryChargeInMW { get; init; }
        public int BatteryDischargeInMW { get; init; }

        public int NumberOfResidentialCustomers { get; init; }
        public int NumberOfCommercialCustomers { get; init; }

        public required SimulationParameters Parameters { get; init; }
    }

    public class SimulationParameters
    {
        public int CloudCoverIncreasePercentage { get; init; }
        public int TemperatureIncreaseDegrees { get; init; }
        public int WindSpeedDecreasePercentage { get; init; }

        public int TraditionalOutputDecreasePercentage { get; init; }
        public int RenewableOutputDecreasePercentage { get; init; }
    }
}