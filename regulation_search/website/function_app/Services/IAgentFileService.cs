namespace Farrellsoft.Examples.Agents.SingleAgent.Services;

public interface IAgentFileService
{
    Task ProcessFileAsync(BinaryData data, string filename);
}
