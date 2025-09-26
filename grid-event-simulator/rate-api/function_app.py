import azure.functions as func
import datetime
import json
import logging
import random

app = func.FunctionApp()

def get_energy_rate():
    """
    Returns a random energy rate between $30-$160.
    """
    # Generate random rate between $30 and $160
    rate = random.uniform(30.0, 160.0)
    logging.info(f'Generated rate: ${rate:.2f}')
    return rate

@app.route(route="get_rate", auth_level=func.AuthLevel.ANONYMOUS)
def get_rate(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Energy rate API called.')
    
    try:
        # Get a random energy rate
        rate = get_energy_rate()
        
        # Create response object
        response_data = {
            "rate": round(rate, 2),
            "currency": "USD",
            "unit": "per MWh",
            "timestamp": datetime.datetime.utcnow().isoformat() + "Z"
        }
        
        return func.HttpResponse(
            json.dumps(response_data),
            status_code=200,
            headers={
                "Content-Type": "application/json"
            }
        )
        
    except Exception as e:
        logging.error(f'Error generating energy rate: {str(e)}')
        error_response = {
            "error": "Internal server error",
            "message": "Unable to generate energy rate"
        }
        return func.HttpResponse(
            json.dumps(error_response),
            status_code=500,
            headers={
                "Content-Type": "application/json"
            }
        )