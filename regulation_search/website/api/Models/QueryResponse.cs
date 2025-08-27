namespace Farrellsoft.Examples.Agents.SingleAgent.Models;

public class QueryResponse
{
    /// <summary>
    /// The response content that can be directly rendered in a chat window.
    /// Supports HTML content and markdown formatting.
    /// </summary>
    public string Response { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional thread identifier for conversation tracking
    /// </summary>
    public string? ThreadId { get; set; }
}
