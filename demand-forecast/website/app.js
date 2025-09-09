// app.js - JavaScript for Weather & Renewable Details page

// Check negotiate endpoint on document ready and update visual indicator
async function checkNegotiateEndpoint() {
  const statusEl = document.getElementById('negotiateStatus');
  const baseUrl = 'https://func-multiagent-eus2-mx01.azurewebsites.net/';
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), 5000);

  try {
    const res = await fetch(`${baseUrl}api/negotiate`, { method: 'POST', signal: controller.signal });
    clearTimeout(timeout);

    if (res.ok) {
      statusEl.innerHTML = `
        <div class="alert alert-success d-flex align-items-center" role="status">
          <span class="badge bg-success rounded-circle me-2" style="width:12px;height:12px;display:inline-block;">&nbsp;</span>
          <div>CONNECTED</div>
        </div>`;
      // Optionally log the negotiate response for debugging
      try { const json = await res.json(); console.debug('negotiate response', json); } catch (e) {}
    } else {
      statusEl.innerHTML = `
        <div class="alert alert-danger d-flex align-items-center" role="alert">
          <span class="badge bg-danger rounded-circle me-2" style="width:12px;height:12px;display:inline-block;">&nbsp;</span>
          <div>Negotiate endpoint returned HTTP ${res.status}</div>
        </div>`;
    }
  } catch (err) {
    clearTimeout(timeout);
    const msg = err.name === 'AbortError' ? 'request timed out' : 'network error or CORS blocked';
    statusEl.innerHTML = `
      <div class="alert alert-danger d-flex align-items-center" role="alert">
        <span class="badge bg-danger rounded-circle me-2" style="width:12px;height:12px;display:inline-block;">&nbsp;</span>
        <div>Negotiate endpoint check failed: ${msg}</div>
      </div>`;
    console.error('Negotiate check error:', err);
  }
}

// Simple client-side behavior: compute dummy renewable values when Send Weather is clicked
function setupWeatherHandler() {
  const sendBtn = document.getElementById('sendWeatherBtn');
  if (!sendBtn) return;

  sendBtn.addEventListener('click', () => {
    const cloud = document.getElementById('cloudCoverage').value;
    // Wind is provided in miles per hour (mph) in the UI; convert to m/s for the toy model
    const windMph = parseFloat(document.getElementById('windSpeed').value) || 0;
    const wind = windMph / 2.2369362920544; // convert mph -> m/s

    // Base assumptions for a toy model (MW)
    const baseSolarClear = 500; // MW when clear
    const baseSolarPartly = 350;
    const baseSolarMostly = 150;
    const baseSolarCloudy = 50;

    let solar = baseSolarClear;
    switch (cloud) {
      case 'clear': solar = baseSolarClear; break;
      case 'partly': solar = baseSolarPartly; break;
      case 'mostly': solar = baseSolarMostly; break;
      case 'cloudy': solar = baseSolarCloudy; break;
    }

    // Wind contribution: assume 10 MW per m/s of wind speed (toy model)
    const windOutput = Math.max(0, wind) * 10;

    const totalRenewable = Math.round((solar + windOutput) * 10) / 10; // MW

    // Assume a fixed grid demand for this demo
    const gridDemand = 1000; // MW

    const percent = Math.round((totalRenewable / gridDemand) * 1000) / 10; // one decimal place

    // Update UI
    document.getElementById('totalRenewableOutput').value = totalRenewable.toLocaleString();
    document.getElementById('totalGridDemand').value = gridDemand.toLocaleString();
    document.getElementById('percentRenewables').value = isFinite(percent) ? percent : '--';
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

// Run setup functions when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
  populateTimeOptions();
  setupWeatherHandler();
  checkNegotiateEndpoint();
});
