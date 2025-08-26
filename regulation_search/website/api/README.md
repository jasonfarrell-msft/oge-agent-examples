Azure Functions (Python) - SendQuery

This folder contains a single Azure Function named `SendQuery` that exposes a POST endpoint at `/api/send`.

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
