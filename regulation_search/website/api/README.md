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

## Development

### Prerequisites
- .NET 8.0 SDK
- Azure Functions Core Tools

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

## Architecture

- **SendFunction**: The main Azure Function that handles HTTP requests
- **IProcessQueryService**: Interface for the query processing service
- **ProcessQueryService**: Implementation that currently echoes the query back
- **QueryRequest/QueryResponse**: Models for request/response data

The service is designed to be easily extensible - you can enhance the `ProcessQueryService` to implement actual query processing logic while maintaining the same interface.
