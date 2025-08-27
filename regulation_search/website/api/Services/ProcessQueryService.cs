using Farrellsoft.Examples.Agents.SingleAgent.Models;
using Azure.AI.Projects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.ClientModel;

namespace Farrellsoft.Examples.Agents.SingleAgent.Services;

public class ProcessQueryService : IProcessQueryService
{
    private readonly AIProjectClient _aiProjectClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessQueryService> _logger;
    private readonly string _agentId;

    public ProcessQueryService(
        AIProjectClient aiProjectClient, 
        IConfiguration configuration,
        ILogger<ProcessQueryService> logger)
    {
        _aiProjectClient = aiProjectClient;
        _configuration = configuration;
        _logger = logger;
        _agentId = _configuration["AgentId"] 
            ?? throw new InvalidOperationException("AgentId configuration is required");
    }

    public async Task<QueryResponse> ProcessQuery(string query, string? threadId = null)
    {
        try
        {
            _logger.LogInformation("Processing query with Azure AI Foundry Agent. ThreadId: {ThreadId}", threadId);

            // Get agents client
            var agentsClient = _aiProjectClient.GetAgentsClient();

            // Get or create thread
            AgentThread thread;
            if (!string.IsNullOrEmpty(threadId))
            {
                _logger.LogInformation("Retrieving existing thread: {ThreadId}", threadId);
                try
                {
                    thread = await agentsClient.GetThreadAsync(threadId);
                }
                catch (ClientResultException ex) when (ex.Status == 404)
                {
                    _logger.LogWarning("Thread {ThreadId} not found, creating new thread", threadId);
                    thread = await agentsClient.CreateThreadAsync();
                }
            }
            else
            {
                _logger.LogInformation("Creating new thread");
                thread = await agentsClient.CreateThreadAsync();
            }

            // Create message in the thread
            var message = await agentsClient.CreateMessageAsync(
                thread.Id,
                MessageRole.User,
                query);

            _logger.LogInformation("Created message in thread {ThreadId}: {MessageId}", thread.Id, message.Value.Id);

            // Create and run the thread with the agent
            var threadRun = await agentsClient.CreateRunAsync(
                thread.Id,
                _agentId);

            _logger.LogInformation("Started run {RunId} for thread {ThreadId}", threadRun.Value.Id, thread.Id);

            // Wait for the run to complete
            do
            {
                await Task.Delay(1000); // Wait 1 second between polls
                threadRun = await agentsClient.GetRunAsync(thread.Id, threadRun.Value.Id);
                _logger.LogDebug("Run status: {Status}", threadRun.Value.Status);
            }
            while (threadRun.Value.Status == RunStatus.Queued || 
                   threadRun.Value.Status == RunStatus.InProgress ||
                   threadRun.Value.Status == RunStatus.RequiresAction);

            if (threadRun.Value.Status != RunStatus.Completed)
            {
                _logger.LogError("Run failed with status: {Status}", threadRun.Value.Status);
                return new QueryResponse
                {
                    Response = $"Agent run failed with status: {threadRun.Value.Status}",
                    ThreadId = thread.Id
                };
            }

            // Get the messages from the thread (the agent's response)
            var messages = await agentsClient.GetMessagesAsync(thread.Id);
            
            // Find the latest assistant message (should be from the agent)
            var latestMessage = messages.Value.Data
                .Where(m => m.Role.ToString().Equals("assistant", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();

            if (latestMessage?.ContentItems?.Any() != true)
            {
                _logger.LogWarning("No response content found from agent");
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
            
            _logger.LogInformation("Received response from agent. Length: {Length}", agentResponse.Length);

            return new QueryResponse
            {
                Response = agentResponse,
                ThreadId = thread.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing query with Azure AI Foundry Agent");
            
            return new QueryResponse
            {
                Response = "An error occurred while processing your query. Please try again.",
                ThreadId = threadId
            };
        }
    }
}
