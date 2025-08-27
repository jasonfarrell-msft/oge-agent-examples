using Farrellsoft.Examples.Agents.SingleAgent.Models;
using Azure.AI.Projects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.ClientModel;
using Azure.Identity;
using Azure.AI.Agents.Persistent;

namespace Farrellsoft.Examples.Agents.SingleAgent.Services;

public class ProcessQueryService(IConfiguration configuration, ILogger<ProcessQueryService> logger) : IProcessQueryService
{
    public async Task<QueryResponse> ProcessQuery(string query, string? threadId = null)
    {
        logger.LogInformation("Processing query with Azure AI Foundry Agent. ThreadId: {ThreadId}", threadId);

        var projectClient = new AIProjectClient(
            endpoint: new Uri("https://foundry-agent-demo-eus2-mx01.services.ai.azure.com/api/projects/singleAgentDemo"),
            credential: new DefaultAzureCredential());

        var agentsClient = projectClient.GetPersistentAgentsClient();
        var agent = await agentsClient.Administration.GetAgentAsync("asst_bDUQeFI6nobYQAs55GAYL3KL");

        // Get or create thread
        PersistentAgentThread thread;
        if (!string.IsNullOrEmpty(threadId))
        {
            logger.LogInformation("Retrieving existing thread: {ThreadId}", threadId);
            try
            {
                thread = await agentsClient.Threads.GetThreadAsync(threadId);
            }
            catch (ClientResultException ex) when (ex.Status == 404)
            {
                logger.LogWarning("Thread {ThreadId} not found, creating new thread", threadId);
                thread = await agentsClient.Threads.CreateThreadAsync();
            }
        }
        else
        {
            logger.LogInformation("Creating new thread");
            thread = await agentsClient.Threads.CreateThreadAsync();
        }

        // Create message in the thread
        var message = await agentsClient.Messages.CreateMessageAsync(
            thread.Id,
            MessageRole.User,
            query);

        logger.LogInformation("Created message in thread {ThreadId}: {MessageId}", thread.Id, message.Value.Id);

        // Create and run the thread with the agent
        var threadRun = await agentsClient.Runs.CreateRunAsync(
            thread.Id,
            agent.Value.Id);

        logger.LogInformation("Started run {RunId} for thread {ThreadId}", threadRun.Value.Id, thread.Id);

        // Wait for the run to complete
        do
        {
            await Task.Delay(1000); // Wait 1 second between polls
            threadRun = await agentsClient.Runs.GetRunAsync(thread.Id, threadRun.Value.Id);
            logger.LogDebug("Run status: {Status}", threadRun.Value.Status);
        }
        while (threadRun.Value.Status == RunStatus.Queued ||
               threadRun.Value.Status == RunStatus.InProgress ||
               threadRun.Value.Status == RunStatus.RequiresAction);

        if (threadRun.Value.Status != RunStatus.Completed)
        {
            logger.LogError("Run failed with status: {Status}", threadRun.Value.Status);
            return new QueryResponse
            {
                Response = $"Agent run failed with status: {threadRun.Value.Status}",
                ThreadId = thread.Id
            };
        }

        // Get the messages from the thread (the agent's response)
        // Messages client returns an AsyncPageable<PersistentThreadMessage> which is not awaitable;
        // enumerate it with await foreach to collect into a list.
        var messagesPaged = agentsClient.Messages.GetMessagesAsync(thread.Id);

        var allMessages = new List<PersistentThreadMessage>();
        await foreach (var msg in messagesPaged)
        {
            allMessages.Add(msg);
        }

        // Find the latest assistant message (should be from the agent)
        var latestMessage = allMessages
            .Where(m => m.Role.ToString().Equals("assistant", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefault();

        if (latestMessage?.ContentItems?.Any() != true)
        {
            logger.LogWarning("No response content found from agent");
            return new QueryResponse
            {
                Response = "No response received from agent",
                ThreadId = thread.Id
            };
        }

        // Extract text content from the message
        var textContent = latestMessage.ContentItems.FirstOrDefault();
        string agentResponse;

        if (textContent is MessageTextContent textMessage)
        {
            agentResponse = textMessage.Text;
        }
        else
        {
            // Fallback: convert content to string
            agentResponse = textContent?.ToString() ?? "Empty response";
        }

        logger.LogInformation("Received response from agent. Length: {Length}", agentResponse.Length);

        return new QueryResponse
        {
            Response = agentResponse,
            ThreadId = threadId
        };
    }
}
