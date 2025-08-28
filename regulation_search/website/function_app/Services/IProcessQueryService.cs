using Farrellsoft.Examples.Agents.SingleAgent.Models;

namespace Farrellsoft.Examples.Agents.SingleAgent.Services;

public interface IProcessQueryService
{
    Task<QueryResponse> ProcessQuery(string query, string? threadId = null);
}
