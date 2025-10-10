import azure.functions as func
import datetime
import json
import logging
import random
import requests
import os

app = func.FunctionApp()

def get_energy_rate():
    """
    Returns energy rate from GridStatus API (SPP LMP real-time 5-min data).
    Takes the LMP value from the first record in the response data array.
    Falls back to random rate if API is unavailable.
    """
    try:
        # GridStatus API endpoint
        dataset_id = 'spp_lmp_real_time_5_min'
        api_url = f"https://api.gridstatus.io/v1/datasets/{dataset_id}/query"
        
        # Get API key from environment variable
        api_key = os.getenv('GRIDSTATUS_API_KEY')
        if not api_key:
            logging.error('GRIDSTATUS_API_KEY environment variable not set')
            raise ValueError('API key not configured')
        
        # Set up headers with API key
        headers = {
            'x-api-key': api_key
        }
        
        # Set up query parameters
        params = {
            'limit': 5
        }
        
        # Make API request with timeout and authentication
        response = requests.get(api_url, headers=headers, params=params, timeout=10)
        response.raise_for_status()
        
        # Parse JSON response
        response_data = response.json()
        
        # Check if response has the expected structure
        if (response_data and 
            'status_code' in response_data and 
            response_data['status_code'] == 200 and
            'data' in response_data and 
            len(response_data['data']) > 0):
            
            # Get the first record from the data array
            first_record = response_data['data'][0]
            
            # Extract the LMP (Locational Marginal Price) value
            if 'lmp' in first_record and first_record['lmp'] is not None:
                rate = float(first_record['lmp'])
                logging.info(f'Retrieved LMP rate from GridStatus API: ${rate:.2f}')
                return rate
            else:
                logging.warning('No valid lmp field found in API response')
        else:
            logging.warning('Invalid response structure from GridStatus API')
            
    except requests.exceptions.RequestException as e:
        logging.error(f'API request failed: {str(e)}')
    except (ValueError, KeyError, TypeError) as e:
        logging.error(f'Error parsing API response: {str(e)}')
    except Exception as e:
        logging.error(f'Unexpected error calling GridStatus API: {str(e)}')
    
    # Fallback to random rate if API fails
    fallback_rate = random.uniform(30.0, 160.0)
    logging.info(f'Using fallback random rate: ${fallback_rate:.2f}')
    return fallback_rate

@app.route(route="get_rate", auth_level=func.AuthLevel.ANONYMOUS)
def get_rate(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Energy rate API called.')
    
    try:
        # Get energy rate from GridStatus API
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