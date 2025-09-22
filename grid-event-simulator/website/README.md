# Grid Event Simulator - Control Panel

A single-page web application for configuring and running power grid simulations.

## Overview

This control panel interface allows users to configure various aspects of a power grid simulation including:

- **Generation Parameters**: Renewable and traditional power output, ramp rates
- **Battery Storage**: Capacity and discharge rates  
- **Demand Configuration**: Residential and commercial customer loads
- **Simulation Parameters**: Environmental and technical conditions

## Files Structure

```
website/
├── index.html          # Main application HTML structure
├── app.css            # All styling and visual design
├── app.js             # Core application logic and functionality
└── README.md          # This documentation file
```

## Features

### Control Panel (Left Side - 40% width)
- **Generation Section**: Configure renewable/traditional output and ramp rates
- **Battery Section**: Set battery capacity and discharge parameters  
- **Demand Section**: Configure residential and commercial customer counts
- **Simulation Parameters**: 
  - Environment subsection (cloud cover, temperature, wind speed)
  - Technical subsection (output decreases and failures)

### Results Panel (Right Side - 60% width)
- White background panel ready for simulation results and visualizations
- Independent scrolling from control panel

### Interactive Elements
- Real-time slider value updates with animations
- Centered "Run Simulation" button below both panels
- Responsive design for different screen sizes
- Professional control panel styling with vibrant subsection headers

## Technical Details

- **Framework**: Bootstrap 5.3.2 for responsive grid system
- **JavaScript**: ES6 classes with modular design
- **CSS**: Custom styling with gradients, animations, and professional appearance
- **Data Management**: All configuration values collected in structured format
- **Persistence**: Local storage for saving simulation configurations

## Usage

1. Open `index.html` in a modern web browser
2. Configure parameters using the sliders in the left control panel
3. Set simulation parameters in the Environment and Technical subsections
4. Click "Run Simulation" to execute (results will appear in right panel)

## Browser Compatibility

- Chrome 60+
- Firefox 55+
- Safari 12+
- Edge 79+

## Version History

- **v2.0.0**: Single-page control panel design (current)
- **v1.x**: Legacy wizard-based interface (deprecated)