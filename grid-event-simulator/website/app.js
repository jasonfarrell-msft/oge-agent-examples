// Wizard state management
let currentStep = 1;
const totalSteps = 3;

// DOM elements
const steps = document.querySelectorAll('.wizard-step');
const progressBar = document.querySelector('.progress-bar');
const stepLabels = document.querySelectorAll('.step-label');

// Form elements - Step 1
const currentOutputSlider = document.getElementById('currentOutput');
const maxOutputSlider = document.getElementById('maxOutput');
const rampRateSlider = document.getElementById('rampRate');

const currentOutputValue = document.getElementById('currentOutputValue');
const maxOutputValue = document.getElementById('maxOutputValue');
const rampRateValue = document.getElementById('rampRateValue');

// Battery sliders - Step 1
const batteryCapacitySlider = document.getElementById('batteryCapacity');
const batteryChargeSlider = document.getElementById('batteryCharge');
const dischargeRateSlider = document.getElementById('dischargeRate');

const batteryCapacityValue = document.getElementById('batteryCapacityValue');
const batteryChargeValue = document.getElementById('batteryChargeValue');
const dischargeRateValue = document.getElementById('dischargeRateValue');

const maxOutputError = document.getElementById('maxOutputError');

// Form elements - Step 2
const residentialSlider = document.getElementById('residential');
const commercialSlider = document.getElementById('commercial');
const temperatureSlider = document.getElementById('temperature');

const residentialValue = document.getElementById('residentialValue');
const commercialValue = document.getElementById('commercialValue');
const temperatureValue = document.getElementById('temperatureValue');

// Form elements - Step 3 (Demand Spike)
const peakTemperatureSlider = document.getElementById('peakTemperature');
const durationSlider = document.getElementById('duration');

const peakTemperatureValue = document.getElementById('peakTemperatureValue');
const durationValue = document.getElementById('durationValue');

// Form elements - Step 3 (Output Reduction)
const outputReductionSlider = document.getElementById('outputReduction');
const outputReductionValue = document.getElementById('outputReductionValue');
const outputReductionDurationSlider = document.getElementById('outputReductionDuration');
const outputReductionDurationValue = document.getElementById('outputReductionDurationValue');

// Navigation buttons
const nextStep1 = document.getElementById('nextStep1');
const prevStep2 = document.getElementById('prevStep2');
const nextStep2 = document.getElementById('nextStep2');
const prevStep3 = document.getElementById('prevStep3');

// Simulation elements
const runSimulationButtons = document.querySelectorAll('.btn-run-simulation');

// Initialize the wizard
document.addEventListener('DOMContentLoaded', function() {
    updateSliderValues();
    setupEventListeners();
    showStep(1);
});

// Setup event listeners
function setupEventListeners() {
    // Step 1 slider value updates
    currentOutputSlider.addEventListener('input', updateCurrentOutputValue);
    maxOutputSlider.addEventListener('input', updateMaxOutputValue);
    rampRateSlider.addEventListener('input', updateRampRateValue);
    batteryCapacitySlider.addEventListener('input', updateBatteryCapacityValue);
    batteryChargeSlider.addEventListener('input', updateBatteryChargeValue);
    dischargeRateSlider.addEventListener('input', updateDischargeRateValue);

    // Step 2 slider value updates
    residentialSlider.addEventListener('input', updateResidentialValue);
    commercialSlider.addEventListener('input', updateCommercialValue);
    temperatureSlider.addEventListener('input', updateTemperatureValue);

    // Step 3 slider value updates
    peakTemperatureSlider.addEventListener('input', updatePeakTemperatureValue);
    durationSlider.addEventListener('input', updateDurationValue);
    outputReductionSlider.addEventListener('input', updateOutputReductionValue);
    outputReductionDurationSlider.addEventListener('input', updateOutputReductionDurationValue);

    // Navigation buttons
    nextStep1.addEventListener('click', () => validateAndNextStep(1));
    prevStep2.addEventListener('click', () => showStep(1));
    nextStep2.addEventListener('click', () => {
        updateBaselineDisplay();
        updatePeakTemperatureMinimum();
        showStep(3);
    });
    prevStep3.addEventListener('click', () => showStep(2));

    // Simulation buttons - single button now controls both tabs
    runSimulationButtons.forEach(button => {
        button.addEventListener('click', handleRunSimulation);
    });
    
    // View Results button
    const viewResultsBtn = document.getElementById('viewResultsBtn');
    if (viewResultsBtn) {
        viewResultsBtn.addEventListener('click', handleViewResults);
    }

    // Real-time validation
    currentOutputSlider.addEventListener('input', validateOutputs);
    maxOutputSlider.addEventListener('input', validateOutputs);
}

// Update slider values
function updateCurrentOutputValue() {
    currentOutputValue.textContent = currentOutputSlider.value;
    validateOutputs();
}

function updateMaxOutputValue() {
    maxOutputValue.textContent = maxOutputSlider.value;
    validateOutputs();
}

function updateRampRateValue() {
    rampRateValue.textContent = rampRateSlider.value;
}

function updateBatteryCapacityValue() {
    batteryCapacityValue.textContent = batteryCapacitySlider.value;
}

function updateBatteryChargeValue() {
    batteryChargeValue.textContent = batteryChargeSlider.value;
}

function updateDischargeRateValue() {
    dischargeRateValue.textContent = dischargeRateSlider.value;
}

// Step 2 slider value update functions
function updateResidentialValue() {
    residentialValue.textContent = parseInt(residentialSlider.value).toLocaleString();
}

function updateCommercialValue() {
    commercialValue.textContent = parseInt(commercialSlider.value).toLocaleString();
}

function updateTemperatureValue() {
    temperatureValue.textContent = temperatureSlider.value;
}

// Step 3 slider value update functions
function updatePeakTemperatureValue() {
    peakTemperatureValue.textContent = peakTemperatureSlider.value;
}

function updateDurationValue() {
    const minutes = parseInt(durationSlider.value);
    if (minutes >= 60) {
        const hours = Math.floor(minutes / 60);
        const remainingMinutes = minutes % 60;
        if (remainingMinutes === 0) {
            durationValue.textContent = `${hours} hour${hours > 1 ? 's' : ''}`;
        } else {
            durationValue.textContent = `${hours}h ${remainingMinutes}m`;
        }
    } else {
        durationValue.textContent = `${minutes} min`;
    }
}

function updateOutputReductionValue() {
    outputReductionValue.textContent = outputReductionSlider.value;
}

function updateOutputReductionDurationValue() {
    const minutes = parseInt(outputReductionDurationSlider.value);
    if (minutes >= 60) {
        const hours = Math.floor(minutes / 60);
        const remainingMinutes = minutes % 60;
        if (remainingMinutes === 0) {
            outputReductionDurationValue.textContent = `${hours} hour${hours > 1 ? 's' : ''}`;
        } else {
            outputReductionDurationValue.textContent = `${hours}h ${remainingMinutes}m`;
        }
    } else {
        outputReductionDurationValue.textContent = `${minutes} min`;
    }
}

function updateSliderValues() {
    // Step 1 values
    updateCurrentOutputValue();
    updateMaxOutputValue();
    updateRampRateValue();
    updateBatteryCapacityValue();
    updateBatteryChargeValue();
    updateDischargeRateValue();
    
    // Step 2 values
    updateResidentialValue();
    updateCommercialValue();
    updateTemperatureValue();
    
    // Step 3 values
    updatePeakTemperatureValue();
    updateDurationValue();
    updateOutputReductionValue();
    updateOutputReductionDurationValue();
}

// Validation functions
function validateOutputs() {
    const currentOutput = parseInt(currentOutputSlider.value);
    const maxOutput = parseInt(maxOutputSlider.value);
    
    if (maxOutput < currentOutput) {
        maxOutputError.style.display = 'block';
        maxOutputSlider.classList.add('is-invalid');
        return false;
    } else {
        maxOutputError.style.display = 'none';
        maxOutputSlider.classList.remove('is-invalid');
        return true;
    }
}

function validateStep1() {
    const isValid = validateOutputs();
    
    if (!isValid) {
        // Shake the form to indicate validation error
        const form = document.getElementById('generationForm');
        form.classList.add('shake');
        setTimeout(() => form.classList.remove('shake'), 500);
    }
    
    return isValid;
}

// Navigation functions
function validateAndNextStep(step) {
    if (step === 1) {
        if (validateStep1()) {
            showStep(2);
        }
    } else {
        showStep(step + 1);
    }
}

function showStep(stepNumber) {
    if (stepNumber < 1 || stepNumber > totalSteps) return;
    
    // Hide all steps
    steps.forEach(step => step.classList.remove('active'));
    stepLabels.forEach(label => label.classList.remove('active'));
    
    // Show current step
    document.getElementById(`step${stepNumber}`).classList.add('active');
    stepLabels[stepNumber - 1].classList.add('active');
    
    // Update progress bar
    const progressPercentage = (stepNumber / totalSteps) * 100;
    progressBar.style.width = `${progressPercentage}%`;
    progressBar.setAttribute('aria-valuenow', progressPercentage);
    
    currentStep = stepNumber;
}

// Update baseline information display
function updateBaselineDisplay() {
    // Update Generation values
    document.getElementById('displayCurrentOutput').textContent = `${currentOutputSlider.value} MW`;
    document.getElementById('displayMaxOutput').textContent = `${maxOutputSlider.value} MW`;
    document.getElementById('displayRampRate').textContent = `${rampRateSlider.value} minutes`;
    document.getElementById('displayBatteryCapacity').textContent = `${batteryCapacitySlider.value} MW`;
    document.getElementById('displayBatteryCharge').textContent = `${batteryChargeSlider.value}%`;
    document.getElementById('displayDischargeRate').textContent = `${dischargeRateSlider.value} MW`;
    
    // Update Demand values
    document.getElementById('displayResidential').textContent = `${parseInt(residentialSlider.value).toLocaleString()} customers`;
    document.getElementById('displayCommercial').textContent = `${parseInt(commercialSlider.value).toLocaleString()} customers`;
    document.getElementById('displayTemperature').textContent = `${temperatureSlider.value}°F`;
}

// Update peak temperature minimum based on current temperature
function updatePeakTemperatureMinimum() {
    const currentTemp = parseInt(temperatureSlider.value);
    peakTemperatureSlider.min = currentTemp;
    
    // If current peak temperature is below the new minimum, update it
    if (parseInt(peakTemperatureSlider.value) < currentTemp) {
        peakTemperatureSlider.value = currentTemp + 5; // Set to slightly above current temp
        updatePeakTemperatureValue();
    }
}

// Handle simulation runs
async function handleRunSimulation(event) {
    const button = event.target;
    const activeTab = document.querySelector('.tab-pane.active');
    const isDemandSpike = activeTab.id === 'demand-spike';
    
    // Get UI elements
    const busyIndicator = document.getElementById('busyIndicator');
    const viewResultsBtn = document.getElementById('viewResultsBtn');
    const allButtons = document.querySelectorAll('button');
    
    try {
        // Disable all buttons and show busy indicator
        allButtons.forEach(btn => btn.disabled = true);
        busyIndicator.classList.remove('d-none');
        viewResultsBtn.classList.add('d-none');
        
        // Clear any previous simulation results
        window.simulationResult = null;
        
        // Build the request payload according to the schema
        const requestData = {
            baseline: {
                current_output: parseInt(currentOutputSlider.value),
                max_output: parseInt(maxOutputSlider.value),
                ramp_rate: parseInt(rampRateSlider.value),
                battery_capacity: parseInt(batteryCapacitySlider.value),
                charge_percent: parseInt(batteryChargeSlider.value),
                discharge_rate: parseInt(dischargeRateSlider.value)
            },
            demand: {
                residential_customers: parseInt(residentialSlider.value),
                commercial_customers: parseInt(commercialSlider.value),
                current_temperature: parseInt(temperatureSlider.value)
            },
            simulation: {}
        };
        
        // Add the appropriate simulation parameters based on active tab
        if (isDemandSpike) {
            requestData.simulation.demand_increase = {
                peak_temperature: parseInt(peakTemperatureSlider.value),
                peak_duration: parseInt(durationSlider.value)
            };
        } else {
            requestData.simulation.output_reduction = {
                reduce_output: parseInt(outputReductionSlider.value),
                duration: parseInt(outputReductionDurationSlider.value)
            };
        }
        
        console.log('Sending simulation request:', requestData);
        
        // Send POST request to the simulation endpoint
        const response = await fetch('https://app-orch-multi-agent-eus2-mx01.azurewebsites.net/RunSimulation', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(requestData)
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        console.log('Simulation result:', result);
        
        // Hide busy indicator and show View Results button
        busyIndicator.classList.add('d-none');
        viewResultsBtn.classList.remove('d-none');
        
        // Store result data for the View Results button
        window.simulationResult = result;
        
    } catch (error) {
        console.error('Simulation request failed:', error);
        
        // Hide busy indicator
        busyIndicator.classList.add('d-none');
        
        // Show error message
        alert('Simulation failed: ' + error.message);
        
    } finally {
        // Re-enable all buttons
        allButtons.forEach(btn => btn.disabled = false);
    }
}

// Handle view results button click
function handleViewResults() {
    if (window.simulationResult) {
        // For now, display results in a modal or alert
        // In a real application, this might navigate to a results page
        alert('Simulation Results:\n\n' + JSON.stringify(window.simulationResult, null, 2));
    } else {
        alert('No simulation results available.');
    }
}

// Add shake animation for validation errors
const style = document.createElement('style');
style.textContent = `
    @keyframes shake {
        0%, 20%, 40%, 60%, 80% {
            transform: translateX(0);
        }
        10%, 30%, 50%, 70%, 90% {
            transform: translateX(-10px);
        }
    }
    
    .shake {
        animation: shake 0.5s;
    }
`;
document.head.appendChild(style);

// Keyboard navigation
document.addEventListener('keydown', function(event) {
    if (event.key === 'ArrowLeft' && currentStep > 1) {
        showStep(currentStep - 1);
    } else if (event.key === 'ArrowRight' && currentStep < totalSteps) {
        if (currentStep === 1) {
            validateAndNextStep(1);
        } else {
            showStep(currentStep + 1);
        }
    } else if (event.key === 'Enter' && currentStep === 1) {
        validateAndNextStep(1);
    }
});

// Export functions for potential use in other scripts
window.WizardApp = {
    showStep,
    validateStep1,
    getCurrentStepData: () => ({
        // Step 1 data
        currentOutput: parseInt(currentOutputSlider.value),
        maxOutput: parseInt(maxOutputSlider.value),
        rampRate: parseInt(rampRateSlider.value),
        batteryCapacity: parseInt(batteryCapacitySlider.value),
        batteryChargePercent: parseInt(batteryChargeSlider.value),
        // Step 2 data
        residential: parseInt(residentialSlider.value),
        commercial: parseInt(commercialSlider.value),
        temperature: parseInt(temperatureSlider.value)
    })
};
