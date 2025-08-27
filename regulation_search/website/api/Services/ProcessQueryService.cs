using Farrellsoft.Examples.Agents.SingleAgent.Models;
using System.Text;

namespace Farrellsoft.Examples.Agents.SingleAgent.Services;

public class ProcessQueryService : IProcessQueryService
{
    public async Task<QueryResponse> ProcessQuery(string query, string? threadId = null)
    {
        return await Task.FromResult(new QueryResponse
        {
            Response = query,
            ThreadId = threadId,
        });
    }
}
