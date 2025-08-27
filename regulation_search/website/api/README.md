Azure Functions (Python) - SendQuery - Python 3.8 Compatible

This folder contains a single Azure Function named `SendQuery` that exposes a POST endpoint at `/api/send`.

## Python 3.8 Compatibility

This project includes compatibility fixes for Python 3.8 runtime issues with the Azure AI Projects SDK.

### Files Added for Compatibility:
- `py38_compat.py` - Contains patches for Python 3.8 compatibility issues
- Updated `requirements.txt` with specific SDK versions
- Enhanced error handling in `agents.py`

### Requirements:
- Python 3.8+ (optimized for Azure Functions runtime)
- Azure AI Projects SDK v1.0.0b3 (more stable for Python 3.8)
- typing-extensions for backported typing features

### Troubleshooting Python 3.8 Issues:

1. **ABCMeta object is not subscriptable error:**
   - This is fixed by the `py38_compat.py` module
   - Ensure it's imported before `azure.ai.projects`

2. **Generic type annotation errors:**
   - Use `typing-extensions` package for better compatibility
   - The compatibility module patches these automatically

3. **MutableMapping subscription errors:**
   - Fixed by patching `collections.abc.MutableMapping` to be subscriptable

### Environment Variables:
Set these in your Azure Static Web App configuration:
- `AZURE_AI_PROJECT_ENDPOINT` - Your Azure AI Foundry project endpoint
- `AZURE_AI_AGENT_ID` - Your agent ID

Usage (local):

1. Create and activate a virtual environment named `.venv`:

   python3 -m venv .venv
   source .venv/bin/activate

2. Install dependencies:

   pip install --upgrade pip
   pip install -r requirements.txt

3. Run the function locally using Azure Functions Core Tools (install separately):

   func start

Notes:
- The function validates that a JSON body with a `query` field is provided; otherwise it returns HTTP 400.
- The function calls `process_query` from `agents.py` — replace that implementation with your real agent logic.
