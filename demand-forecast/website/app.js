// app.js - JavaScript for Weather & Renewable Details page

// Simple client-side behavior: compute dummy renewable values when Set Weather is clicked
function setupWeatherHandler() {
  console.log('Setting up weather handler...');
  const setWeatherBtn = document.getElementById('setWeatherBtn');
  console.log('Weather button element:', setWeatherBtn);
  if (!setWeatherBtn) {
    console.error('Set Weather button not found!');
    return;
  }

  setWeatherBtn.addEventListener('click', async () => {
    console.log('setting weather');
    // Clear any previous error messages
    hideWeatherStatus();
    
    // Show progress indicator and disable button
    showWeatherProgress();
    setWeatherBtn.disabled = true;
    
    try {
      // Map cloud coverage to numbers
      const cloudCoverageValue = document.getElementById('cloudCoverage').value;
      let cloudCoverageNumber;
      switch (cloudCoverageValue) {
        case 'clear': cloudCoverageNumber = 0; break;
        case 'partly': cloudCoverageNumber = 1; break;
        case 'mostly': cloudCoverageNumber = 2; break;
        case 'cloudy': cloudCoverageNumber = 3; break;
        default: cloudCoverageNumber = 0; break;
      }
      
      // Prepare payload for API call
      const timeOfDayValue = document.getElementById('timeOfDay').value || '12:00';
      const payload = {
        windSpeed: parseFloat(document.getElementById('windSpeed').value) || 0,
        cloudCoverage: cloudCoverageNumber,
        timeOfDay: timeOfDayValue
      };
      
      console.log('Sending weather payload:', payload);
      
      // Make actual API call
      await postWeatherData(payload);
      
      // Show success message
      showWeatherSuccess();
      
    } catch (error) {
      console.error('Weather API call failed:', error);
      showWeatherError();
    } finally {
      // Re-enable button and hide progress
      setWeatherBtn.disabled = false;
      hideWeatherProgress();
    }
  });
}

// Populate the timeOfDay select with 24-hour options at 15-minute increments
function populateTimeOptions() {
  const select = document.getElementById('timeOfDay');
  if (!select) return;
  select.innerHTML = '';
  for (let h = 0; h < 24; h++) {
    for (const m of [0, 15, 30, 45]) {
      const hh = String(h).padStart(2, '0');
      const mm = String(m).padStart(2, '0');
      const val = `${hh}:${mm}`;
      const opt = document.createElement('option');
      opt.value = val;
      opt.textContent = val;
      select.appendChild(opt);
    }
  }

  // Set a sensible default: current time rounded to nearest 15 minutes (24-hour)
  const now = new Date();
  const rounded = Math.round(now.getMinutes() / 15) * 15;
  if (rounded === 60) {
    now.setHours(now.getHours() + 1);
    now.setMinutes(0);
  } else {
    now.setMinutes(rounded);
  }
  const defaultVal = `${String(now.getHours() % 24).padStart(2, '0')}:${String(now.getMinutes()).padStart(2, '0')}`;
  if ([...select.options].some(o => o.value === defaultVal)) select.value = defaultVal;
}

// Demand status management functions
function hideDemandStatus() {
  document.getElementById('demandSaved').style.display = 'none';
  document.getElementById('demandError').style.display = 'none';
  document.getElementById('demandProgress').style.display = 'none';
}

function showDemandProgress() {
  hideDemandStatus();
  document.getElementById('demandProgress').style.display = 'inline';
}

function hideDemandProgress() {
  document.getElementById('demandProgress').style.display = 'none';
}

function showDemandSuccess() {
  hideDemandStatus();
  const savedSpan = document.getElementById('demandSaved');
  savedSpan.style.display = 'inline';
  // Auto-hide success message after 5 seconds (consistent with weather)
  setTimeout(() => {
    savedSpan.style.display = 'none';
  }, 5000);
}

function showDemandError() {
  hideDemandStatus();
  document.getElementById('demandError').style.display = 'inline';
  // Error message stays visible until next button press
}

// Run status management functions
function hideRunStatus() {
  document.getElementById('runError').style.display = 'none';
  document.getElementById('renewableOverlay').style.display = 'none';
}

function showRunProgress() {
  hideRunStatus();
  const overlay = document.getElementById('renewableOverlay');
  overlay.style.display = 'flex';
}

function hideRunProgress() {
  document.getElementById('renewableOverlay').style.display = 'none';
}

function showRunSuccess() {
  hideRunStatus();
  // No success message needed - just hide overlay and show populated data
}

function showRunError() {
  hideRunStatus();
  // Clear all field values
  document.getElementById('percentRenewables').value = '--';
  document.getElementById('totalGridDemand').value = '--';
  document.getElementById('totalRenewableOutput').value = '--';
  // Show error message
  document.getElementById('runError').style.display = 'inline';
}

// Weather status management functions
function hideWeatherStatus() {
  document.getElementById('weatherSaved').style.display = 'none';
  document.getElementById('weatherError').style.display = 'none';
  document.getElementById('weatherProgress').style.display = 'none';
}

function showWeatherProgress() {
  hideWeatherStatus();
  document.getElementById('weatherProgress').style.display = 'inline';
}

function hideWeatherProgress() {
  document.getElementById('weatherProgress').style.display = 'none';
}

function showWeatherSuccess() {
  hideWeatherStatus();
  const savedSpan = document.getElementById('weatherSaved');
  savedSpan.style.display = 'inline';
  // Auto-hide success message after 5 seconds
  setTimeout(() => {
    savedSpan.style.display = 'none';
  }, 5000);
}

function showWeatherError() {
  hideWeatherStatus();
  document.getElementById('weatherError').style.display = 'inline';
  // Error message stays visible until next button press
}

// Post weather data to the API endpoint
async function postWeatherData(payload) {
  const response = await fetch('https://func-multiagent-eus2-mx01.azurewebsites.net/api/receive_weather', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(payload)
  });

  if (!response.ok) {
    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
  }

  // Server only returns status code, no JSON content
  console.log('Weather data sent successfully:', payload, 'Status:', response.status);
  return { status: response.status };
}

// Post demand data to the API endpoint
async function postDemandData(payload) {
  const response = await fetch('https://func-multiagent-eus2-mx01.azurewebsites.net/api/receive_demand', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(payload)
  });

  if (!response.ok) {
    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
  }

  // Server only returns status code, no JSON content
  console.log('Demand data sent successfully:', payload, 'Status:', response.status);
  return { status: response.status };
}

// Execute the renewable calculation
async function executeRenewableCalculation() {
  const response = await fetch('https://func-multiagent-eus2-mx01.azurewebsites.net/api/execute', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
  }

  // This endpoint returns JSON data
  const result = await response.json();
  console.log('Execute response:', result);
  return result;
}

// Handler for Set Demand button
function setupDemandHandler() {
  const setDemandBtn = document.getElementById('setDemandBtn');
  if (!setDemandBtn) return;

  setDemandBtn.addEventListener('click', async () => {
    // Clear any previous error messages
    hideDemandStatus();
    
    // Show progress indicator and disable button
    showDemandProgress();
    setDemandBtn.disabled = true;
    
    try {
      const temperature = parseFloat(document.getElementById('temperatureF').value) || 70;
      const numCustomers = parseInt(document.getElementById('numCustomers').value) || 1000;
      
      // Prepare payload for API call
      const payload = {
        numberOfCustomers: numCustomers,
        temperature: temperature
      };
      
      console.log('Sending demand payload:', payload);
      
      // Make actual API call
      await postDemandData(payload);
      
      // Show success message
      showDemandSuccess();
      
    } catch (error) {
      console.error('Demand API call failed:', error);
      showDemandError();
    } finally {
      // Re-enable button and hide progress
      setDemandBtn.disabled = false;
      hideDemandProgress();
    }
  });
}

// Handler for Run button
function setupRunHandler() {
  const runBtn = document.getElementById('runBtn');
  if (!runBtn) return;

  runBtn.addEventListener('click', async () => {
    // Clear any previous error messages and show overlay
    showRunProgress();
    runBtn.disabled = true;
    
    try {
      console.log('Executing renewable calculation...');
      
      // Make API call
      const result = await executeRenewableCalculation();
      
      // Update the renewable details fields with response data
      if (result.percentRenewables !== undefined) {
        // Convert decimal (0..1) to percentage and format to 1 decimal place
        const raw = Number(result.percentRenewables);
        const bounded = Number.isFinite(raw) ? Math.min(Math.max(raw, 0), 1) : 0;
        const percentValue = (bounded * 100).toFixed(1);
        document.getElementById('percentRenewables').value = percentValue;
      }
      
      if (result.totalGridNeeds !== undefined) {
        const val = Number(result.totalGridNeeds);
        document.getElementById('totalGridDemand').value = Number.isFinite(val) ? val.toLocaleString() : '--';
      }
      
      if (result.totalRenewableOutput !== undefined) {
        const val2 = Number(result.totalRenewableOutput);
        document.getElementById('totalRenewableOutput').value = Number.isFinite(val2) ? val2.toLocaleString() : '--';
      }
      
      // Success - just hide overlay (no message needed)
      showRunSuccess();
      
    } catch (error) {
      console.error('Execute API call failed:', error);
      showRunError();
    } finally {
      // Hide progress overlay and re-enable button
      hideRunProgress();
      runBtn.disabled = false;
    }
  });
}

// Run setup functions when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
  console.log('DOM Content Loaded - starting setup...');
  populateTimeOptions();
  setupWeatherHandler();
  setupDemandHandler();
  setupRunHandler();
  // Ensure overlay is hidden on page load
  hideRunStatus();
  console.log('Setup complete');
});
