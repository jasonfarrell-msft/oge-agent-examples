using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Farrellsoft.Examples.Agents.MultiAgent.Services
{
    public class DefaultAgentService(IConfiguration configuration, ILogger<DefaultAgentService> logger) : IAgentService
    {
        public async Task<AnalysisResult> ExecuteAnalysis()
        {
            var projectClient = new AIProjectClient(
                endpoint: new Uri(configuration["FoundryProjectEndpoint"] ?? throw new InvalidOperationException("FoundryProjectEndpoint not configured")),
                credential: new DefaultAzureCredential());

            var agentsClient = projectClient.GetPersistentAgentsClient();
            var thread = (await agentsClient.Threads.CreateThreadAsync()).Value;

            var message = await agentsClient.Messages.CreateMessageAsync(
                thread.Id,
                MessageRole.User,
                "Run analysis. Return only the JSON document in the response");

            var response = await GetResponse(agentsClient, thread.Id,
                configuration["AgentId"] ?? throw new InvalidOperationException("AgentId not configured"));

            if (string.IsNullOrEmpty(response))
                throw new Exception("Agent did not return a response");

            var analysisResult = JsonSerializer.Deserialize<AnalysisResult>(response);
            if (analysisResult == null)
                throw new Exception("Failed to deserialize agent response");

            return analysisResult;
        }

        async Task<string> GetResponse(PersistentAgentsClient agentsClient, string threadId, string agentId)
        {
            var threadRun = await agentsClient.Runs.CreateRunAsync(
                threadId,
                agentId);

            do
            {
                await Task.Delay(1000); // Wait 1 second between polls
                threadRun = await agentsClient.Runs.GetRunAsync(threadId, threadRun.Value.Id);
                logger.LogDebug("Run status: {Status}", threadRun.Value.Status);
            }
            while (threadRun.Value.Status == RunStatus.Queued ||
                threadRun.Value.Status == RunStatus.InProgress ||
                threadRun.Value.Status == RunStatus.RequiresAction);

            if (threadRun.Value.Status != RunStatus.Completed)
                throw new Exception($"Agent run failed with status: {threadRun.Value.Status}");

            var messagesPaged = agentsClient.Messages.GetMessagesAsync(threadId);

            var allMessages = new List<PersistentThreadMessage>();
            await foreach (var msg in messagesPaged)
            {
                allMessages.Add(msg);
            }

            return allMessages.SelectMany(x => x.ContentItems)
                .Where(x => x is MessageTextContent)
                .Cast<MessageTextContent>()
                .Select(x => x.Text)
                .LastOrDefault() ?? string.Empty;
        }
    }

    public interface IAgentService
    {
        Task<AnalysisResult> ExecuteAnalysis();
    }
}
