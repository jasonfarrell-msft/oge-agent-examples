import azure.functions as func
import datetime
import json
import logging

import random

app = func.FunctionApp()

@app.route(route="weather", methods=["GET"])
def weather_endpoint(req: func.HttpRequest) -> func.HttpResponse:
	# Generate random weather data
	cloud_cover = random.randint(0, 100)  # percentage
	wind_speed = round(random.uniform(0, 40), 1)  # mph
	temperature = round(random.uniform(30, 100), 1)  # Fahrenheit
	# Get current UTC time and adjust for EST (UTC-5)
	now_utc = datetime.datetime.now(datetime.timezone.utc)
	est_time = now_utc - datetime.timedelta(hours=5)
	time_of_day = est_time.strftime("%Y-%m-%d %H:%M:%S EST")
	data = {
		"cloud_cover": cloud_cover,
		"wind_speed": wind_speed,
		"temperature": temperature,
		"time_of_day": time_of_day
	}
	return func.HttpResponse(
		json.dumps(data),
		mimetype="application/json",
		status_code=200
	)