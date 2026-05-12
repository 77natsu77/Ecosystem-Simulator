using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Structs;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Diagnostics;
 using Microsoft.AspNetCore.SignalR;

namespace Ecosystem_Simulator.UI
{
    // A strict blueprint so System.Text.Json doesn't get confused
    // Made this for entities and didnt make specific DTOs for critters/predators because they share so many properties, and the frontend can just check the "Type" field to see which is which. If we wanted to add more entity types in the future, we could just add more optional fields to this DTO and populate them as needed.
    // Note: This is separate from StatsEntry because StatsEntry contains some data we don't want to send to the frontend every frame (like timestamps and counts), and also doesn't contain any entity-specific data, which we need for drawing them on the canvas
    

    public class HeadlessRunner
    {
        private World _world;
        private List<StatsEntry> _historyList = new List<StatsEntry>();
        private StatisticsManager _stats_manager = new StatisticsManager(null); // We will set the world in the stats manager later, but we need to initialize it here to avoid null reference exceptions when trying to save stats before the first log
        private double _internalTimestamp = 0;
        private float _secondsElapsed = 0;
        private bool _isRunning = true;
        private List<EntityExportDTO> exportEntities = new List<EntityExportDTO>(); // This is what we'll serialize and send to the frontend
        
        private IHubContext<WorldHub> _hub; // This is how we'll send data to the frontend in real-time using SignalR
        public HeadlessRunner(World world, IHubContext<WorldHub> hub)
        {
            _world = world;
            _hub = hub;
            _stats_manager = new StatisticsManager(world); // Now that we have the world, we can initialize the stats manager properly
        }


        public void Start()
        {
            Console.WriteLine("Simulation started. Open http://localhost:5000/ to view.");

            int delayMs = (int)(Settings.TickRate * 1000);
            if (delayMs <= 0) delayMs = 16;

            while (_isRunning)
            {
                try
                {
                    _world.Tick(Settings.TickRate);
                   // Console.WriteLine($"Tick: {_internalTimestamp:F2}s, Entities: {_world.Entities.Count}");
                    ProcessFrame(); // fire-and-forget is ok for 20 FPS
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CRASH DURING TICK: {ex.Message}");
                }

                Thread.Sleep(delayMs);
            }
        }

        // might break this up into smaller methods in the future, but for now it works fine as is. This is where we gather all the data we want to send to the frontend every frame, and also where we handle the stats logging and saving to CSV/HTML. It's a bit of a "kitchen sink" method right now, but it works for our purposes. In the future, we might want to refactor it into smaller methods if it gets too unwieldy.
        private  async Task ProcessFrame() // This is where we gather all the data we want to send to the frontend every frame, and also where we handle the stats logging and saving to CSV/HTML. It's a bit of a "kitchen sink" method right now, but it works for our purposes. In the future, we might want to refactor it into smaller methods if it gets too unwieldy.
        {
            // Obtaining and Rendering statistics
            int critterCount = 0, foodCount = 0, predatorCount = 0;
            float sumCritterEnergy = 0, sumCritterSpeed = 0, sumCritterSight = 0, sumCritterMetab = 0, sumCritterRepro = 0;
            float sumPredatorEnergy = 0, sumPredatorSpeed = 0, sumPredatorSight = 0, sumPredatorMetab = 0, sumPredatorRepro = 0;

            // Pre-calculate these inverse values to save some CPU time in the loop, since division is more expensive than multiplication and we do it for every critter/predator every frame. This is a small optimization but can add up with large populations.
            float invCritterEnergy = 1f / Settings.CritterStartingEnergy;
            float invPredatorEnergy = 1f / Settings.PredatorStartingEnergy;
            exportEntities.Clear(); // Clear the list before adding new entities

            for (int i = _world.Entities.Count - 1; i >= 0; i--)// Iterating backwards just in case we need to remove any entities in the future (currently we don't, but it's a common pattern to avoid issues with modifying a list while iterating)
            {
                var entity = _world.Entities[i]; 
                if (entity is Critter c)
                {
                    critterCount++;
                    sumCritterEnergy += c.Energy;
                    sumCritterSpeed += c.Speed;
                    sumCritterSight += c.SightRadius;
                    sumCritterMetab += c.MetabolismEfficiency;
                    sumCritterRepro += c.ReproductionThreshold;
                    float eRatio = Math.Clamp(c.Energy * invCritterEnergy, 0, 1);
                    
                    exportEntities.Add(new EntityExportDTO {
                        Id = c.Id,
                        Type = "Critter", X = c.Position.X, Y = c.Position.Y,
                        Size = 3 + (eRatio * 7), R = (int)((1 - eRatio) * 255), G = 60, B = (int)(eRatio * 255),
                        Sight = c.SightRadius, VelX = c.Velocity.X, VelY = c.Velocity.Y, Speed = c.Speed, Energy = c.Energy
                    });
                }
                else if (entity is Smarty s)
                {
                    //TODO: must correct this later
                    critterCount++;
                    sumCritterEnergy += s.Energy;
                    sumCritterSpeed += s.Speed;
                    sumCritterSight += s.SightRadius;
                    sumCritterMetab += s.MetabolismEfficiency;
                    sumCritterRepro += s.ReproductionThreshold;
                    float eRatio = Math.Clamp(s.Energy * invCritterEnergy, 0, 1);
                    
                    exportEntities.Add(new EntityExportDTO {
                        Id = s.Id,
                        Type = "Smarty", X = s.Position.X, Y = s.Position.Y,
                        Size = 4 + (eRatio * 8), R = (int)((1 - eRatio) * 255), G = 120, B = (int)(eRatio * 255),
                        Sight = s.SightRadius, VelX = s.Velocity.X, VelY = s.Velocity.Y, Speed = s.Speed, Energy = s.Energy,
                        IsScanning = s.IsScanning
                    });
                }
                else if (entity is Predator p)
                {
                    predatorCount++;
                    sumPredatorEnergy += p.Energy;
                    sumPredatorSpeed += p.Speed;
                    sumPredatorSight += p.SightRadius;
                    sumPredatorMetab += p.MetabolismEfficiency;
                    sumPredatorRepro += p.ReproductionThreshold;
                    float eRatio = Math.Clamp(invPredatorEnergy * p.Energy, 0, 1);
                    
                    exportEntities.Add(new EntityExportDTO {
                        Id = p.Id,
                        Type = "Predator", X = p.Position.X, Y = p.Position.Y,
                        Size = 3 + (eRatio * 7), R = (int)(255), G = (int)((1 - eRatio) * 255), B = (int)(eRatio * 255),
                        Sight = p.SightRadius, Cannibal = p.CannibalMode, VelX = p.Velocity.X, VelY = p.Velocity.Y, Speed = p.Speed, Energy = p.Energy
                        
                    });
                }
                else if (entity is FoodPellet f)
                {
                    foodCount++;
                    exportEntities.Add(new EntityExportDTO {
                        Id = f.Id,
                        Type = "Food", X = f.Position.X, Y = f.Position.Y, Energy = f.EnergyValue,
                        Size = 4, R = 255, G = 255, B = 255
                    });
                }
            }

            double lastLogTime = _historyList.Count > 0 ? _historyList.Last().Timestamp : -30; // -30 ensures it logs on frame 1
            _internalTimestamp +=Settings.TickRate;

            if (_internalTimestamp - lastLogTime >= Settings.StatsSaveRate) // Save data every StatsSaveRate seconds, independent of TickRate (which can be changed for performance reasons and doesn't need to affect the stats logging)
            {
                // calculate averages
                float AverageCritterEnergy = critterCount > 0 ? sumCritterEnergy / critterCount : 0;
                float AverageCritterSpeed = critterCount > 0 ? sumCritterSpeed / critterCount : 0;
                float AverageCritterSightRadius = critterCount > 0 ? sumCritterSight / critterCount : 0;
                float AverageCritterMetabolismEfficiency = critterCount > 0 ? sumCritterMetab / critterCount : 0;
                float AverageCritterReproductionThreshold = critterCount > 0 ? sumCritterRepro / critterCount : 0;
                float AveragePredatorEnergy = predatorCount > 0 ? sumPredatorEnergy / predatorCount : 0;
                float AveragePredatorSpeed = predatorCount > 0 ? sumPredatorSpeed / predatorCount : 0;
                float AveragePredatorSightRadius = predatorCount > 0 ? sumPredatorSight / predatorCount : 0;
                float AveragePredatorMetabolismEfficiency = predatorCount > 0 ? sumPredatorMetab / predatorCount : 0;
                float AveragePredatorReproductionThreshold = predatorCount > 0 ? sumPredatorRepro / predatorCount : 0;

                _secondsElapsed += Settings.StatsSaveRate; // ensures timestamps saved are precise and consistent
                StatsEntry EntityInfo = new StatsEntry // data for graphs and csv files
                {
                    Timestamp = _secondsElapsed,
                    CritterCount = critterCount,
                    FoodCount = foodCount,
                    PredatorCount = predatorCount,
                    CritterAvgSight = AverageCritterSightRadius,
                    CritterAvgEnergy = AverageCritterEnergy,
                    CritterAvgSpeed = AverageCritterSpeed,
                    CritterAvgMetabolismEfficiency = AverageCritterMetabolismEfficiency,
                    CritterAvgReproductionThreshold = AverageCritterReproductionThreshold,
                    PredatorAvgSight = AveragePredatorSightRadius,
                    PredatorAvgEnergy = AveragePredatorEnergy,
                    PredatorAvgSpeed = AveragePredatorSpeed,
                    PredatorAvgMetabolismEfficiency = AveragePredatorMetabolismEfficiency,
                    PredatorAvgReproductionThreshold = AveragePredatorReproductionThreshold
                };

                _historyList.Add(EntityInfo); // Add the new stats entry to the history list, which is used for both CSV and HTML exports. This way we keep a complete history of all the stats entries, and we can optimize the HTML export later to only update the latest entry instead of rewriting the whole file every time.
                _stats_manager.RecordStatistics(EntityInfo);
                HTMLExporter.ExportToHTML(_historyList); // Needs work, currently it just overwrites the same html file every time with the full history, which is inefficient and causes performance issues as the history grows. We need to either optimize the HTMLExporter to only update the data in the existing file without rewriting the whole thing, or we need to implement a different system for storing and accessing the stats data for graphing (like a lightweight database or in-memory data structure that the frontend can query). For now, we'll just rely on the CSV file for stats analysis and leave the HTMLExporter as a future improvement.
            } 

            var frameData = new
            {
                Width = _world.Width,
                Height = _world.Height,
                ShowArrows = Settings.DISPLAY_VELOCITY_ARROWS,
                Stats = new // This is the data that gets sent to the frontend every frame to be displayed on screen
                {
                    Critters = critterCount,
                    Predators = predatorCount,
                    Food = foodCount
                    // got rid of averages every frame to save CPU, we can get that data from the stats page if needed
                },
                Entities = exportEntities
            };

            //  Serialize data and send the frame data to all connected clients via SignalR, which is much more efficient than writing to a file every frame and having the frontend read from it. The frontend can just listen for "frame" events from the SignalR hub and update the visualization accordingly, which should result in a much smoother experience overall.
            string jsonString = JsonSerializer.Serialize(frameData);
            await _hub.Clients.All.SendAsync("frame", frameData); 
        }  
}}