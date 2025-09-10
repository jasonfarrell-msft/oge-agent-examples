import azure.functions as func
import datetime
import json
import logging
import os
import redis

app = func.FunctionApp()

@app.function_name(name="weather_endpoint")
@app.route(route="weather", methods=["GET"], auth_level=func.AuthLevel.ANONYMOUS)
def weather_endpoint(req: func.HttpRequest) -> func.HttpResponse:
    try:
        # Get Redis connection string from settings
        redis_connection_string = os.environ.get("RedisConnectionString")
        if not redis_connection_string:
            return func.HttpResponse(
                json.dumps({"error": "Redis connection string not found"}),
                mimetype="application/json",
                status_code=500
            )
        
        # Connect to Redis
        r = redis.from_url(redis_connection_string)
        
        # Get current weather from Redis
        weather_json = r.get("CurrentWeather")
        if weather_json is None:
            return func.HttpResponse(
                json.dumps({"error": "Current weather data not found"}),
                mimetype="application/json",
                status_code=404
            )
        
        # Parse the JSON string and return it
        if isinstance(weather_json, bytes):
            weather_data = json.loads(weather_json.decode('utf-8'))
        return func.HttpResponse(
            json.dumps(weather_data),
            mimetype="application/json",
            status_code=200
        )
    
    except redis.RedisError as e:
        return func.HttpResponse(
            json.dumps({"error": f"Redis error: {str(e)}"}),
            mimetype="application/json",
            status_code=500
        )
    except json.JSONDecodeError as e:
        return func.HttpResponse(
            json.dumps({"error": f"Invalid JSON in weather data: {str(e)}"}),
            mimetype="application/json",
            status_code=500
        )
    except Exception as e:
        return func.HttpResponse(
            json.dumps({"error": f"Unexpected error: {str(e)}"}),
            mimetype="application/json",
            status_code=500
        )

@app.function_name(name="demand_endpoint")
@app.route(route="demand", auth_level=func.AuthLevel.ANONYMOUS)
def demand(req: func.HttpRequest) -> func.HttpResponse:
    try:
        # Get Redis connection string from settings
        redis_connection_string = os.environ.get("RedisConnectionString")
        if not redis_connection_string:
            return func.HttpResponse(
                json.dumps({"error": "Redis connection string not found"}),
                mimetype="application/json",
                status_code=500
            )
        
        # Connect to Redis
        r = redis.from_url(redis_connection_string)
        
        # Get current demand from Redis
        demand_json = r.get("currentDemand")
        if demand_json is None:
            return func.HttpResponse(
                json.dumps({"error": "Current demand data not found"}),
                mimetype="application/json",
                status_code=404
            )
        
        # Convert Redis response to string and return as JSON
        demand_str = demand_json.decode('utf-8') if isinstance(demand_json, bytes) else str(demand_json)
        demand_data = json.loads(demand_str)
        return func.HttpResponse(
            json.dumps(demand_data),
            mimetype="application/json",
            status_code=200
        )
    
    except redis.RedisError as e:
        return func.HttpResponse(
            json.dumps({"error": f"Redis error: {str(e)}"}),
            mimetype="application/json",
            status_code=500
        )
    except json.JSONDecodeError as e:
        return func.HttpResponse(
            json.dumps({"error": f"Invalid JSON in demand data: {str(e)}"}),
            mimetype="application/json",
            status_code=500
        )
    except Exception as e:
        return func.HttpResponse(
            json.dumps({"error": f"Unexpected error: {str(e)}"}),
            mimetype="application/json",
            status_code=500
        )