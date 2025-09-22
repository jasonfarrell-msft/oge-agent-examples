/**
 * Grid Event Simulator - Control Panel Application
 * Single-page control panel for power grid simulation configuration and execution
 * @version 2.0.0
 * @author Grid Simulation Team
 */

/**
 * Main GridSimulator class - handles all application functionality
 * Manages slider controls, data collection, simulation execution, and UI interactions
 */
class GridSimulator {
    /**
     * Initialize the GridSimulator instance
     */
    constructor() {
        this.simulationData = new Map();
        this.initialize();
    }

    /**
     * Set up DOM event listeners and initialize the application
     */
    initialize() {
        document.addEventListener('DOMContentLoaded', () => {
            this.initializeApplication();
        });
    }

    initializeApplication() {
        console.log('Initializing Grid Event Simulator');
        
        // Initialize all sliders and controls
        this.initializeSliders();
        
        // Initialize the run simulation button
        this.initializeRunButton();
        
        // Load any saved data
        this.loadSavedData();
    }

    initializeSliders() {
        // Initialize all sliders with proper event listeners
        const sliders = document.querySelectorAll('.form-range');
        sliders.forEach(slider => {
            const valueDisplayId = slider.id.replace(/(-output|-capacity|-rate|-customers|-cover|-increase|-time|-drop|-decrease)$/, '') + '-value';
            const valueDisplay = document.getElementById(valueDisplayId) || 
                                document.getElementById(slider.id + '-value');
            
            if (valueDisplay) {
                // Set initial values
                valueDisplay.textContent = slider.value;
                this.updateSliderBackground(slider);
                
                // Add event listeners
                slider.addEventListener('input', () => {
                    valueDisplay.textContent = slider.value;
                    this.updateSliderBackground(slider);
                    this.animateValueChange(valueDisplay);
                });
            }
        });
    }

    initializeRunButton() {
        const runButton = document.getElementById('run-simulation-btn');
        if (runButton) {
            runButton.addEventListener('click', () => {
                this.runSimulation();
            });
        }
    }

    runSimulation() {
        const button = document.getElementById('run-simulation-btn');
        if (!button) return;
        
        // Collect all current configuration data
        const simulationConfig = this.collectAllData();
        
        // Show loading state
        const originalText = button.innerHTML;
        button.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Running Simulation...';
        button.disabled = true;
        
        // Simulate processing time
        setTimeout(() => {
            // Reset button state
            button.innerHTML = originalText;
            button.disabled = false;
            
            // Save simulation data
            this.saveSimulationData(simulationConfig);
            
            // Show success message
            this.showNotification('Simulation completed successfully!', 'success');
            
            // Log the data for debugging
            console.log('Simulation Configuration:', simulationConfig);
            
        }, 2000);
    }

    collectAllData() {
        return {
            renewables: {
                output: this.getSliderValue('renewables-output', 800)
            },
            traditional: {
                output: this.getSliderValue('traditional-output', 1000),
                rampRate: this.getSliderValue('ramp-rate', 30)
            },
            battery: {
                capacity: this.getSliderValue('battery-capacity', 2000),
                dischargeRate: this.getSliderValue('discharge-rate', 270)
            },
            demand: {
                residential: this.getSliderValue('residential-customers', 1750),
                commercial: this.getSliderValue('commercial-customers', 550)
            },
            simulationParameters: {
                environment: {
                    cloudCover: this.getSliderValue('cloud-cover', 0),
                    temperatureIncrease: this.getSliderValue('temperature-increase', 0),
                    windSpeedDrop: this.getSliderValue('wind-speed-drop', 0)
                },
                technical: {
                    traditionalDecrease: this.getSliderValue('traditional-decrease', 0),
                    renewableDrop: this.getSliderValue('renewable-drop', 0)
                }
            },
            timestamp: new Date().toISOString()
        };
    }

    getSliderValue(sliderId, defaultValue = 0) {
        const slider = document.getElementById(sliderId);
        return slider ? parseInt(slider.value) : defaultValue;
    }

    saveSimulationData(data) {
        this.simulationData.set('latest', data);
        localStorage.setItem('gridSimulatorLatest', JSON.stringify(data));
    }

    loadSavedData() {
        // Load any previously saved simulation data
        const saved = localStorage.getItem('gridSimulatorLatest');
        if (saved) {
            try {
                const data = JSON.parse(saved);
                this.simulationData.set('latest', data);
                console.log('Loaded saved simulation data:', data);
            } catch (error) {
                console.warn('Error loading saved data:', error);
            }
        }
    }

    updateSliderBackground(slider) {
        const value = slider.value;
        const min = slider.min;
        const max = slider.max;
        const percentage = ((value - min) / (max - min)) * 100;
        
        slider.style.background = `linear-gradient(to right, #007bff 0%, #007bff ${percentage}%, #e9ecef ${percentage}%, #e9ecef 100%)`;
    }

    animateValueChange(element) {
        element.classList.add('value-change');
        setTimeout(() => {
            element.classList.remove('value-change');
        }, 300);
    }

    // Shared utility functions
    showNotification(message, type = 'info') {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        notification.style.cssText = `
            top: 20px;
            right: 20px;
            z-index: 9999;
            max-width: 400px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        `;
        
        notification.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        // Add to page
        document.body.appendChild(notification);
        
        // Auto-remove after 5 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 5000);
    }
}

// Initialize the grid simulator
const gridSimulator = new GridSimulator();

// Export global utilities
window.GridSimulator = {
    showNotification: (message, type) => gridSimulator.showNotification(message, type),
    runSimulation: () => gridSimulator.runSimulation(),
    getSimulationData: () => gridSimulator.simulationData.get('latest'),
    collectAllData: () => gridSimulator.collectAllData()
};