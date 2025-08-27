import logging
import json
import azure.functions as func

from agents import process_query

def main(req: func.HttpRequest) -> func.HttpResponse:
    """HTTP POST endpoint at /api/send that requires a JSON body containing 'query'.

    Validates the presence of 'query' in the request JSON and calls agents.process_query.
    Returns 400 if validation fails, 500 on processing errors, and 200 with JSON result on success.
    """
    logging.info("SendQuery: received request")

    try:
        req_body = req.get_json()
    except ValueError:
        return func.HttpResponse(
            "Request body must be valid JSON.", status_code=400
        )

    if not isinstance(req_body, dict):
        return func.HttpResponse(
            "Request JSON must be an object.", status_code=400
        )

    if "query" not in req_body:
        return func.HttpResponse(
            "Missing required field 'query' in request body.", status_code=400
        )

    query_value = req_body["query"]
    thread_id = req_body.get("thread_id")

    try:
        if thread_id is not None:
            result = process_query(query_value, thread_id)
        else:
            result = process_query(query_value)
    except Exception:
        logging.exception("Error while processing query")
        return func.HttpResponse(
            json.dumps({"error": "internal server error"}),
            status_code=500,
            mimetype="application/json",
        )

    # Ensure the result is JSON serializable
    try:
        body = json.dumps(result)
    except TypeError:
        body = json.dumps({"result": str(result)})

    return func.HttpResponse(body, status_code=200, mimetype="application/json")
