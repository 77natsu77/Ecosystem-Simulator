# Ecosystem Simulator (C#)  
[![CI](https://github.com/77natsu77/Ecosystem-Simulator/workflows/CI/badge.svg)](https://github.com/77natsu77/Ecosystem-Simulator/actions)

## Overview
A high‑performance, headless C# ecosystem simulator that models predator–prey dynamics using genetic algorithms and streams live state to a browser UI via SignalR. Built as part of my preparation for Software Engineering Degree applications, this project focuses on algorithmic efficiency, data visualization, and modern headless architecture.

## Features
- **Headless Architecture:** C# simulation engine runs independently from the HTML5 Canvas frontend.
- **SignalR Real‑Time Streaming:** Live frames are pushed directly to the browser (no file I/O jitter).
- **Dual‑Species Evolution:** Genetic traits mutate across speed, sight, metabolism, and reproduction.
- **Predator AI:** Includes adaptive hunting logic and cannibal mode under starvation.
- **Algorithmic Efficiency:** Custom spatial hashing enables O(1) neighbor lookups.
- **Data Pipeline:** Statistics exported to CSV with Chart.js visualizations + Python analytics.

## Technical Approach
Naive neighbor detection is O(n²), which is impractical at scale. This simulator uses spatial hashing to reduce lookup to ~O(1).  
The project evolved from a WinForms UI to a **headless + web** architecture, where the C# engine acts as a fast computation core, and the browser receives live frames over SignalR.

## Visualizations & Analytics
Live sim view in browser plus historical graphs:
- `index.html` – live HTML5 canvas view  
- `Exports/population_over_time.html` – population trends  
- `Exports/critter_data_over_time.html` – trait evolution  
- `Exports/predator_data_over_time.html` – predator adaptation  

### Simulation
**Current (Stream-based WebSockets(HTML5 + SignalR))**  
https://github.com/user-attachments/assets/02d7dddd-4ec5-4a3e-bc12-e3de2647de99

**Previous (File based json)**
https://github.com/user-attachments/assets/f572b674-0209-487c-bc96-823249f29f60

**Previous (Winforms)**
https://github.com/user-attachments/assets/bf675d37-6fd8-48d3-8529-52602ea6620f


### HTML Graphs
<img width="744" height="401" alt="Population Graph" src="https://github.com/user-attachments/assets/f223635f-e7b8-4fc5-926e-6f6a8db3e5ca"/>
<img width="744" height="401" alt="Critter Traits Graph" src="https://github.com/user-attachments/assets/df874aa5-2469-43a2-be09-6aa1cbe0ad0b"/>
<img width="744" height="401" alt="Predator Traits Graph" src="https://github.com/user-attachments/assets/83779b1b-636f-4cdd-a2ac-06bf61257c1a"/>

## How to Run
This project runs in GitHub Codespaces or local VS Code.

1. Clone the repository and open in VS Code.  
2. Ensure **.NET 10 SDK** is installed.  
3. Press **F5** (or Run/Debug).  
4. The simulation starts and hosts a SignalR server.  
5. Open `http://localhost:5000` to view the live simulation.

(Optional) Run Python scripts in `/Analytics` for deeper CSV analysis.

## Testing
The project includes 35 xUnit tests covering genome mutation, energy policies, and spatial hashing. CI runs on GitHub Actions.

## Architecture
- **Core/** – Simulation engine, genomes, policies, statistics  
- **Entities/** – Critter, Predator, FoodPellet  
- **Environment/** – World + spatial hashing  
- **UI/** – HeadlessRunner + SignalR frame streaming  
- **Analytics/** – Python scripts for CSV processing  
- **Exports/** – CSV logs and HTML charts  

Design patterns used:
- Strategy Pattern (energy policies)  
- Template Pattern (genome abstraction)  
- Observer Pattern (event-driven spawning)  

## What’s Next
- Delta‑compression for frame updates (reduce payload size).
- Parallelize `Tick()` for large populations.
- Add seasonal dynamics and terrain variations.
- Expand to additional species types.

## Tech Stack
- C# 12 / .NET 10  
- SignalR (real‑time WebSocket streaming)  
- HTML5 Canvas & JavaScript  
- Python 3 (Pandas, Matplotlib)  
- Chart.js  
- xUnit  
- GitHub Actions CI/CD