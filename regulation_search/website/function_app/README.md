# Azure Static Web App Backend API

This is a .NET backend for an Azure Static Web App that provides a single POST endpoint for processing queries.

## Project Structure

```
api/
├── Models/
│   ├── QueryRequest.cs     # Request model with query and optional thread_id
│   └── QueryResponse.cs    # Response model with response and thread_id
├── Services/
│   ├── IProcessQueryService.cs    # Service interface
│   └── ProcessQueryService.cs     # Service implementation
├── SendFunction.cs         # Azure Function that handles /api/send
├── Program.cs             # Dependency injection configuration
├── api.csproj            # Project file
├── host.json             # Azure Functions host configuration
└── local.settings.json   # Local development settings
```

## API Endpoint

### POST /api/send

Processes a query request and returns a response.

**Request Body:**
```json
{
  "query": "your query here",        // Required: The query to process
  "threadId": "optional-thread-id"   // Optional: Thread identifier
}
```

**Response (200 OK):**
```json
{
  "response": "<div class=\"chat-response\">...HTML content...</div>",  // HTML-formatted response ready for chat display
  "threadId": "optional-thread-id",   // The thread ID if provided
  "contentType": "html",              // Content type (html, markdown, or text)
  "timestamp": "2025-08-27T10:30:00Z", // When the response was generated
  "isComplete": true                  // Whether the response is complete
}
```

The `response` field contains HTML-formatted content that can be directly inserted into a chat window using `innerHTML`. The response includes:
- Proper HTML structure with CSS classes for styling
- Escaped user input to prevent XSS attacks
- Rich formatting (bold, italic, lists, links, etc.)
- Timestamp display
- Structured layout suitable for chat interfaces

**Error Responses:**
- `400 Bad Request`: When the query field is missing, empty, or the JSON is invalid
- `500 Internal Server Error`: When an unexpected error occurs

## Configuration

The API requires the following configuration settings in `local.settings.json` for local development or as Application Settings in Azure:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "FoundryProjectEndpoint": "https://your-foundry-project.services.ai.azure.com/api/projects/yourProject",
    "AgentId": "asst_your_agent_id"
  }
}
```

### Authentication

The API uses `DefaultAzureCredential` for authentication with Azure AI Foundry. This supports:
- Managed Identity (in Azure)
- Azure CLI credentials (local development)
- Visual Studio credentials (local development)
- Environment variables (AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, AZURE_TENANT_ID)

For local development, ensure you're logged in with Azure CLI:
```bash
az login
```

## Development

### Prerequisites
- .NET 8.0 SDK
- Azure Functions Core Tools
- Azure CLI (for authentication)
- Access to an Azure AI Foundry project with a configured agent

### Running Locally
```bash
# Restore packages
dotnet restore

# Build the project
dotnet build

# Run the Azure Functions locally
func start
```

The API will be available at `http://localhost:7071/api/send`

### Testing the Endpoint

```bash
# Valid request
curl -X POST http://localhost:7071/api/send \
  -H "Content-Type: application/json" \
  -d '{"query": "Hello, world!", "threadId": "12345"}'

# Response:
# {
#   "response": "<div class=\"chat-response\">...<p>You asked: <em>\"Hello, world!\"</em></p>...</div>",
#   "threadId": "12345",
#   "contentType": "html",
#   "timestamp": "2025-08-27T10:30:00Z",
#   "isComplete": true
# }

# Invalid request (missing query)
curl -X POST http://localhost:7071/api/send \
  -H "Content-Type: application/json" \
  -d '{"threadId": "12345"}'

# Response: 400 Bad Request
# Field 'query' is required and cannot be empty
```

### Chat Integration

The response is designed to be directly usable in chat interfaces:

```javascript
// Fetch response from API
const response = await fetch('/api/send', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ query: "Hello!", threadId: "123" })
});

const data = await response.json();

// Directly insert into chat window
document.getElementById('chat-messages').innerHTML += data.response;
```

See `example-chat-integration.html` for a complete working example.

## Deployment

This backend is designed to work with Azure Static Web Apps. The Azure Functions will be automatically deployed as part of the Static Web App deployment process.

### Processing Logic

The `ProcessQueryService` now integrates with Azure AI Foundry:

1. **Thread Management**: 
   - If a `threadId` is provided, it retrieves the existing conversation thread
   - If no `threadId` is provided or the thread doesn't exist, it creates a new thread
   
2. **Agent Interaction**:
   - Posts the user query as a message to the thread
   - Creates a run with the configured Azure AI Agent
   - Waits for the agent to process and respond
   
3. **Response Formatting**:
   - Extracts the agent's response from the thread
   - Formats it as HTML suitable for chat display
   - Returns the response along with the thread ID for conversation continuity

### Error Handling

The service includes comprehensive error handling:
- Missing or invalid configuration
- Azure AI service connectivity issues
- Agent execution failures
- Missing or malformed responses

All errors are logged and return user-friendly error messages in the chat format.
