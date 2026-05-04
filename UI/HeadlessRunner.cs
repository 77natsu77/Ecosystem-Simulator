using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Diagnostics;

namespace Ecosystem_Simulator.UI
{
    // A strict blueprint so System.Text.Json doesn't get confused
    // Made this for entities and didnt make specific DTOs for critters/predators because they share so many properties, and the frontend can just check the "Type" field to see which is which. If we wanted to add more entity types in the future, we could just add more optional fields to this DTO and populate them as needed.
    // Note: This is separate from StatsEntry because StatsEntry contains some data we don't want to send to the frontend every frame (like timestamps and counts), and also doesn't contain any entity-specific data, which we need for drawing them on the canvas
    public struct EntityExportDTO
    {
        public string Type { get; set; } = "";
        public float X { get; set; }
        public float Y { get; set; }
        public float Size { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public float Sight { get; set; }
        public float VelX { get; set; }
        public float VelY { get; set; }
        public float Speed { get; set; }
        public bool Cannibal { get; set; }

        public EntityExportDTO(string type, float x, float y, float size = 5, int r = 255, int g = 255, int b = 255, float sight = 0, float velX = 0, float velY = 0, float speed = 0, bool cannibal = false)
        {
            Type = type;
            X = x;
            Y = y;
            Size = size;
            R = r;
            G = g;
            B = b;
            Sight = sight;
            VelX = velX;
            VelY = velY;
            Speed = speed;
            Cannibal = cannibal;
        }
    }

    public class HeadlessRunner
    {
        private World _world;
        private List<StatsEntry> _historyList = new List<StatsEntry>();
        private StatisticsManager _stats_manager = new StatisticsManager();
        private double _internalTimestamp = 0;
        private float _secondsElapsed = 0;
        private bool _isRunning = true;
        private Process _webServerProcess; // We'll use Python's built-in HTTP server to serve the frontend files, which is simple and reliable. This Process object allows us to kill the server when the C# app closes.
        private List<EntityExportDTO> exportEntities = new List<EntityExportDTO>(); // This is what we'll serialize and send to the frontend
        public HeadlessRunner(World world)
        {
            _world = world;
        }

        public void Start()
        {
            try {
            // Attempt to kill any process using port 8000 (to prevent crashes if the previous instance didn't close properly). This is a Linux/Mac command; on Windows, users will have to manually ensure port 8000 is free.
            Process.Start("fuser", "-k 8000/tcp")?.WaitForExit();} 
            catch { /* Ignore if fuser isn't found */ }
            Console.WriteLine("Simulation started. Open index.html to view.");
            
            int delayMs = (int)(Settings.TickRate * 1000);
            if (delayMs <= 0) delayMs = 16; // Failsafe if TickRate is broken
            StartWebServer();
            while (_isRunning)
            {
                try
                {
                    _world.Tick(Settings.TickRate);
                    ProcessFrame();
                }
                catch (Exception ex)
                {
                    // If the game crashes, it will appear in terminal, but the web server will keep running so you can inspect the last state
                    Console.WriteLine($"CRASH DURING TICK: {ex.Message}");
                }
                
                Thread.Sleep(delayMs); 
            }
        }

        private void ProcessFrame()
        {
            // Obtaining and Rendering statistics
            int critterCount = 0, foodCount = 0, predatorCount = 0;
            float sumCritterEnergy = 0, sumCritterSpeed = 0, sumCritterSight = 0, sumCritterMetab = 0, sumCritterRepro = 0;
            float sumPredatorEnergy = 0, sumPredatorSpeed = 0, sumPredatorSight = 0, sumPredatorMetab = 0, sumPredatorRepro = 0;

            // Pre-calculate these inverse values to save some CPU time in the loop, since division is more expensive than multiplication and we do it for every critter/predator every frame. This is a small optimization but can add up with large populations.
            float invCritterEnergy = 1f / Settings.CritterStartingEnergy;
            float invPredatorEnergy = 1f / Settings.PredatorStartingEnergy;
            exportEntities.Clear(); // Clear the list before adding new entities

            for (int i = _world.Entities.Count - 1; i >= 0; i--)
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
                        Type = "Critter", X = c.Position.X, Y = c.Position.Y,
                        Size = 3 + (eRatio * 7), R = (int)((1 - eRatio) * 255), G = 60, B = (int)(eRatio * 255),
                        Sight = c.SightRadius, VelX = c.Velocity.X, VelY = c.Velocity.Y, Speed = c.Speed
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
                        Type = "Predator", X = p.Position.X, Y = p.Position.Y,
                        Size = 3 + (eRatio * 7), R = (int)((1 - eRatio) * 255), G = 150, B = (int)(eRatio * 255),
                        Sight = p.SightRadius, Cannibal = p.CannibalMode, VelX = p.Velocity.X, VelY = p.Velocity.Y, Speed = p.Speed
                    });
                }
                else if (entity is FoodPellet f)
                {
                    foodCount++;
                    exportEntities.Add(new EntityExportDTO {
                        Type = "Food", X = f.Position.X, Y = f.Position.Y
                    });
                }
            }


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

            double lastLogTime = _historyList.Count > 0 ? _historyList.Last().Timestamp : -30; // -30 ensures it logs on frame 1
            _internalTimestamp +=Settings.TickRate;
            
            if (_internalTimestamp - lastLogTime >= Settings.StatsSaveRate) // Save data every StatsSaveRate seconds, independent of TickRate (which can be changed for performance reasons and doesn't need to affect the stats logging)
            {
                _secondsElapsed += Settings.StatsSaveRate; // ensures timestamps saved are precise and consistent
                StatsEntry history = new StatsEntry // data for graphs and csv files
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

                _historyList.Add(history);
                _stats_manager.SaveStatsToCSV(_historyList);
                _stats_manager.ExportToHTML(_historyList);
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
                    Food = foodCount,
                    CEnergy = AverageCritterEnergy.ToString("N2"),
                    CSpeed = AverageCritterSpeed.ToString("N2"),
                    CSight = AverageCritterSightRadius.ToString("N2"),
                    CMetab = AverageCritterMetabolismEfficiency.ToString("N2"),
                    CRepro = AverageCritterReproductionThreshold.ToString("N2"),
                    PEnergy = AveragePredatorEnergy.ToString("N2"),
                    PSpeed = AveragePredatorSpeed.ToString("N2"),
                    PSight = AveragePredatorSightRadius.ToString("N2"),
                    PMetab = AveragePredatorMetabolismEfficiency.ToString("N2"),
                    PRepro = AveragePredatorReproductionThreshold.ToString("N2")

                },
                Entities = exportEntities
            };


            string rootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            string filePath = Path.Combine(rootPath, "world_state.json");
            string tempFilePath = Path.Combine(rootPath, "world_state_temp.json");

            //  Serialize data
            string jsonString = JsonSerializer.Serialize(frameData);

            //  Write to the temporary file first
            File.WriteAllText(tempFilePath, jsonString);
            
            //  Swap the files instantly (the 'true' allows overwriting)
            // This method prevents the frontend from ever trying to read the file while it's being written to, which can (happened to me alot) cause crashes or corrupted data
            File.Move(tempFilePath, filePath, true);
        }

        

        private void StartWebServer()
        {
            Task.Run(() => {
                try {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "python3",
                        Arguments = "-m http.server 8000",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."))
                    };

                    _webServerProcess = Process.Start(startInfo);
                    Console.WriteLine("--- Internal Web Server Started on Port 8000 ---");
                    
                    // Ensure that if the C# app is killed, the Python app is killed too (or try to anyways, we don't want orphan processes)
                    AppDomain.CurrentDomain.ProcessExit += (s, e) => _webServerProcess?.Kill();
                    
                    _webServerProcess.WaitForExit();
                }
                catch (Exception ex) {
                    Console.WriteLine($"Web Server failed: {ex.Message}");
                }
            });
        }
    }

    

    
}