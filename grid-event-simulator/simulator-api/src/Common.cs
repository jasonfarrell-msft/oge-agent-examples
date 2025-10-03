namespace GridSimulator.Api;

public enum SimulationType
{
    DemandSpike,
    OutputReduction
}

public static class Constants
{
    public const string DemandCalcKey = "demandCalc";
    public const string OutputCalcKey = "outputCalc";
}