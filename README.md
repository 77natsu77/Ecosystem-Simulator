# Ecosystem Simulator (C#)  
![CI Status](<CI_BADGE_URL>)

## Overview
This project is a C#-based ecosystem simulator that models predator–prey dynamics using a genetic algorithm. Built as part of my preparation for Software Engineering Degree Apprenticeships, it focuses on performance optimisation, clean architecture, and data-driven analysis. The simulation evolves two interacting species—Critters and Predators—over time, producing exportable datasets and visualisations to analyse emergent behaviour.

## Features
- Dual-species system with independent genomes and behaviours  
- Predator hunting logic with adaptive “cannibal mode” under low energy  
- Genetic algorithm with mutation across four traits (speed, sight, metabolism, reproduction)  
- Spatial hashing for efficient neighbour lookup  
- Real-time statistics tracking (10 metrics logged per second)  
- CSV export and interactive Chart.js visualisations  
- JSON-based save/load for full simulation persistence  

## Technical Approach
A key focus of this project is algorithmic efficiency and system design. Naive neighbour detection scales poorly (O(n²)), which becomes impractical as populations grow. To address this, spatial hashing partitions entities into grid cells, reducing lookup complexity to near O(1). This mirrors real-world engineering trade-offs where performance constraints drive architectural decisions.

The predator–prey model reflects feedback systems similar to those seen in financial markets—where agents compete for limited resources and adapt over time. By simulating these dynamics, the project demonstrates understanding of emergent systems, a concept relevant to areas such as quantitative modelling and distributed systems.

## Visualisations
Simulation data is exported and rendered using Chart.js:
- `population_over_time.html` – population trends  
- `critter_data_over_time.html` – trait evolution  
- `predator_data_over_time.html` – predator adaptation  

![Simulation Screenshot](<SCREENSHOT_URL>)

## How to Run
1. Clone the repository  
2. Open in Visual Studio (.NET Framework 4.7.2)  
3. Build and run the WinForms application  
4. Exported data and visualisations are generated during runtime  

## Testing
The project includes 35 unit tests written with xUnit, covering core simulation logic such as genome mutation, energy policies, and spatial hashing behaviour. Continuous Integration is configured via GitHub Actions.

## Architecture
- **Core/** – simulation engine, genomes, policies, statistics  
- **Entities/** – Critter, Predator, FoodPellet  
- **Environment/** – World management and spatial hashing  
- **UI/** – WinForms rendering layer  

Design patterns used:
- Strategy Pattern (energy policies)  
- Template Pattern (genome abstraction)  
- Observer Pattern (event-driven spawning)  

## What’s Next
- Transition to .NET Core for cross-platform support  
- Introduce parallel processing for scalability  
- Expand simulation complexity (additional species, environmental factors)  

## Tech Stack
- C# / .NET Framework 4.7.2  
- WinForms  
- Chart.js  
- xUnit  
- GitHub Actions  