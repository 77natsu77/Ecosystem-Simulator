# Ecosystem Simulator (C#)  
[![CI](https://github.com/77natsu77/Ecosystem-Simulator/workflows/CI/badge.svg)](https://github.com/77natsu77/Ecosystem-Simulator/actions)

## Overview
This project is a high-performance C#-based ecosystem simulator that models predator–prey dynamics using a genetic algorithm. Built as part of my preparation for Software Engineering Degree Apprenticeships, it focuses on performance optimization, clean architecture, and data-driven analysis. The simulation evolves two interacting species—Critters and Predators—over time, running on a headless C# engine that streams data to a real-time HTML5 web frontend.

## Features
- **Decoupled Architecture:** A headless C# simulation engine operating independently from the HTML5 Canvas visualization layer.
- **Dual-Species Evolution:** Independent genomes and behaviors with genetic algorithms mutating four traits (speed, sight, metabolism, reproduction).  
- **Predator AI:** Adaptive hunting logic, including a "cannibal mode" triggered under low energy starvation conditions.  
- **Algorithmic Efficiency:** Custom spatial hashing for O(1) neighbor lookups, bypassing the traditional O(n²) bottlenecks of entity collision.
- **Atomic I/O Streaming:** Safely streams simulation state to a built-in Python web server at 60 FPS without read/write thread locks.
- **Data Pipeline:** Real-time statistics tracking exported to CSV, paired with interactive Chart.js visualizations and Python pandas analysis.

## Technical Approach
A key focus of this project is algorithmic efficiency and system design. Naive neighbor detection scales poorly (O(n²)), which becomes impractical as populations grow. To address this, spatial hashing partitions entities into grid cells, reducing lookup complexity to near O(1). 

Furthermore, migrating from a coupled GUI (WinForms) to a Headless/Web architecture demonstrates modern backend engineering. The C# engine acts purely as a fast number-cruncher, atomically writing state data which is served via a background Python HTTP server to an agnostic web client. This mirrors real-world distributed systems where performance constraints drive architectural decisions.

## Visualizations & Analytics
The simulation can be viewed live in the browser, while historical data is exported and rendered using Chart.js:
- `index.html` – Live, real-time HTML5 Canvas simulation rendering  
- `Exports/population_over_time.html` – Population trends  
- `Exports/critter_data_over_time.html` – Trait evolution  
- `Exports/predator_data_over_time.html` – Predator adaptation  

![Simulation Screenshot](<SCREENSHOT_URL>)

## How to Run
This project is designed to run seamlessly in GitHub Codespaces or any local VS Code environment.

1. Clone the repository and open it in VS Code.
2. Ensure you have the .NET 10 SDK and Python 3 installed.
3. Press **F5** (or select "Run and Debug"). The C# engine will automatically compile, start the simulation, and spin up a background Python web server on Port 8000.
4. Your browser will automatically open to `http://localhost:8000` to view the live simulation.
5. (Optional) Run the Python scripts in the `/Analytics` folder to perform deep-dive analysis on the generated CSV data.

## Testing
The project includes 35 unit tests written with xUnit, covering core simulation logic such as genome mutation, energy policies, and spatial hashing behavior. Continuous Integration is configured via GitHub Actions to ensure build stability on every push.

## Architecture
- **Core/** – Simulation engine, genomes, policies, statistics  
- **Entities/** – Critter, Predator, FoodPellet  
- **Environment/** – World management and spatial hashing  
- **UI/** – HeadlessRunner orchestrating the engine and web server
- **Analytics/** – Python scripts for processing CSV datasets  
- **Exports/** – Generated JSON state files, CSV logs, and HTML charts  

Design patterns used:
- Strategy Pattern (energy policies)  
- Template Pattern (genome abstraction)  
- Observer Pattern (event-driven spawning)  

## What’s Next
- Transition from atomic file writing to WebSockets for zero-allocation memory streaming.
- Introduce multithreading/parallel processing for the `Tick()` update loop.
- Expand simulation complexity (e.g., seasons, varying terrain, new species).

## Tech Stack
- C# 12 / .NET 10
- HTML5 Canvas & JavaScript
- Python 3 (http.server, Pandas, Matplotlib)
- Chart.js  
- xUnit  
- GitHub Actions CI/CD