"""agents.py

Azure AI Foundry integration for processing queries.
"""

import os
import sys
from typing import Any, Optional

# Import Python 3.8 compatibility fixes BEFORE azure.ai.projects
try:
    import py38_compat  # This applies necessary patches
except ImportError:
    pass

try:
    from azure.ai.projects import AIProjectClient
    from azure.identity import DefaultAzureCredential
except ImportError as e:
    if sys.version_info < (3, 9):
        raise ImportError(
            f"Azure AI Projects SDK import failed on Python {sys.version_info.major}.{sys.version_info.minor}. "
            "This is likely due to Python 3.8 compatibility issues. "
            "Try using azure-ai-projects==1.0.0b3 or earlier version. "
            f"Original error: {e}"
        )
    else:
        raise

def get_ai_project_client() -> AIProjectClient:
    """
    Establish connection to Azure AI Foundry project using environment variables.
    
    Expected environment variables:
    - AZURE_AI_PROJECT_ENDPOINT: The Azure AI Foundry project endpoint
    - AZURE_AI_PROJECT_NAME: The Azure AI Foundry project name (optional)
    """
    endpoint = "https://foundry-agent-demo-eus2-mx01.services.ai.azure.com/api/projects/singleAgentDemo"
    
    # Use DefaultAzureCredential for authentication
    credential = DefaultAzureCredential()
    
    # Create the AI Project client
    client = AIProjectClient(
        endpoint=endpoint,
        credential=credential
    )
    
    return client


def process_query(query: str, thread_id: Optional[str] = None) -> Any:
    """
    Process the incoming query using Azure AI Foundry agent.
    
    Args:
        query: The user query to process
        thread_id: Optional thread ID. If not provided, a new thread will be created
        
    Returns:
        JSON-serializable result containing the response and thread information
    """
    try:
        # Get the AI project client
        client = get_ai_project_client()
        
        # Get agent ID from environment variables
        agent_id = "asst_bDUQeFI6nobYQAs55GAYL3KL"
        
        # Get the agent client
        agent_client = client.agents.get_agent(agent_id=agent_id)
        
        # Handle thread management
        if thread_id:
            # Use existing thread
            try:
                thread = client.agents.threads.get(thread_id)
                created_new_thread = False
            except Exception:
                # If thread doesn't exist, create a new one
                thread = client.agents.threads.create()
                thread_id = thread.id
                created_new_thread = True
        else:
            # Create new thread
            thread = client.agents.threads.create()
            thread_id = thread.id
            created_new_thread = True
        
        # Add message to thread
        message = client.agents.messages.create(
            thread_id=thread_id,
            role="user",
            content=query
        )
        
        # Create and run the thread with the agent
        run = client.agents.runs.create(
            thread_id=thread_id,
            agent_id=agent_id
        )
        
        # Wait for the run to complete and get the response
        while run.status in ["queued", "in_progress", "running"]:
            import time
            time.sleep(1)
            run = client.agents.runs.get(thread_id=thread_id, run_id=run.id)
        
        # Get the response messages
        messages = client.agents.messages.list(thread_id=thread_id)
        
        # Extract the latest assistant response
        assistant_response = None
        for msg in messages:
            if msg.role == "assistant" and hasattr(msg, 'created_at') and msg.created_at > message.created_at:
                if msg.content and len(msg.content) > 0:
                    # Extract content as string, handling various content types
                    try:
                        content_item = msg.content[0]
                        # Use getattr to safely access potentially missing attributes
                        if hasattr(content_item, 'text'):
                            text_obj = getattr(content_item, 'text', None)
                            if text_obj and hasattr(text_obj, 'value'):
                                assistant_response = getattr(text_obj, 'value', str(text_obj))
                            else:
                                assistant_response = str(text_obj)
                        elif hasattr(content_item, 'value'):
                            assistant_response = getattr(content_item, 'value', str(content_item))
                        else:
                            assistant_response = str(content_item)
                    except (AttributeError, IndexError, TypeError):
                        assistant_response = str(msg.content)
                break
        
        if not assistant_response:
            assistant_response = "No response received from agent"
        
        return {
            "query": query,
            "response": assistant_response,
            "thread_id": thread_id,
        }
        
    except Exception as e:
        return {
            "error": str(e),
            "message": "Failed to process query with Azure AI Foundry agent",
            "thread_id": thread_id if 'thread_id' in locals() else None,
            "status": "error"
        }
