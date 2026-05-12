using Ecosystem_Simulator.Core.Structs;

namespace Ecosystem_Simulator.Core
{
    public static class Settings //TODO: ENSURE ALL SETTINGS ARE ORGANISED IN AN APPROPRIATE MANNER
    {

        public static int NextEntityId = 0; // Global counter for assigning unique IDs to entities
        // RNG
        public static readonly Random Rng = new Random();

        // WORLD SETTINGS
        public const float WorldWidth = 2300f;
        public const float WorldHeight = 1700f;

        // GRID SETTINGS
        public const float CellSize = 150f;
        // Large prime numbers for hashing
        public const int HashConstantX = 73856093; 
        public const int HashConstantY = 19349663; 
        
        // SAVE SETTINGS
        // file(s) settings
        private static readonly string RootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
    
        private static readonly string ExportsFolder = Path.Combine(RootPath, "Exports");

        public static readonly string WorldSaveFile = Path.Combine(ExportsFolder, "WorldData.json"); 
        public static readonly string CSVStatsFile = Path.Combine(ExportsFolder, "Simulation stats.csv");
        public static readonly string PopulationHTMLFile = Path.Combine(ExportsFolder, "population_over_time.html"); 
        public static readonly string CritterDataHTMLFile = Path.Combine(ExportsFolder, "critter_data_over_time.html");
        public static readonly string PredatorDataHTMLFile = Path.Combine(ExportsFolder, "predator_data_over_time.html");
                            

        // colors for html files
        public const string SpeedColor = "#FF5733"; // Red-Orange
        public const string SightRadiusColor = "#33C1FF"; // Light Blue
        public const string MetabolismEfficiencyColor = "#75FF33"; // Light Green
        public const string ReproductionThresholdColor = "#FF33A8"; // Pink
        public const string EnergyColor = "#FFD700"; // Gold
        public const string CritterPopulationColor = "#8E44AD"; // Purple
        public const string FoodPelletPopulationColor = "#2ECC71"; // Green
        public const string PredatorPopulationColor = "#000000"; // Black

        // other save file settings
        public const int StatsSaveRate = 10; // seconds, the rate at which data is saved to files(this does not include world data which is saved at the very end)
        
        //UI SETTINGS
        public const double TickRate = 0.05; // changing fps to around 20 as opposed to 60 makes the simulation run much smoother and more stable, especially as the population grows and there are more entities to process each frame. It also reduces the CPU load significantly, which is important for keeping the simulation running smoothly on a wider range of hardware. Plus, with the real-time updates to the frontend using SignalR, we can still have a visually appealing and responsive experience even at a lower tick rate.
        public const bool DISPLAY_VELOCITY_ARROWS = true; // whether to display velocity arrows above entities, which can be helpful for visualizing their movement and behavior, but can also clutter the screen and reduce performance if there are many entities, so it's optional based on user preference and hardware capabilities

        // SIMULATION BALANCE SETTINGS
        // TODO: make a single eat distance variable, i see no point in making seperate ones as of right now
        public const float EatDistance = 10f;
        public const float CollisionDistance = 4.5f; // if two entities are within this distance, they will be considered colliding and will adjust their positions to avoid overlap
        public const float BaseMetabolism = 1.67f;

        // INITIALIZATION SETTINGS
        public const int InitialCritterNumber = 60;
        public const int InitialPredatorNumber = 15;
        public const int InitialFoodPelletNumber = 500;
        public const int InitialSmartyNumber = 3;
        

        // ENTITIES SETTINGS
        // CRITTER SETTINGS //
        // Starting values
        public const float StartingCritterSightRadius = 55f;
        public const float StartingCritterSpeed = 125f;
        public const float StartingCritterMetabolismEfficiency = 0.0055f; 
        public const float StartingCritterReproductionThreshold = CritterStartingEnergy * 1.375f;
        

        // Max gene values
        public const float MaxCritterSpeed = StartingCritterSpeed * 2;
        public const float MaxCritterSightradius = StartingCritterSightRadius * 2;
        public const float MaxCritterMetabolismEfficiency = StartingCritterMetabolismEfficiency / 2; 
        public const float MaxCritterReproductionThreshold = StartingCritterReproductionThreshold / 2; 
        
        // Min gene values
        public const float MinCritterSpeed = StartingCritterSpeed / 2;
        public const float MinCritterSightradius = StartingCritterSightRadius / 2;
        public const float MinCritterMetabolismEfficiency = StartingCritterMetabolismEfficiency * 2; 
        public const float MinCritterReproductionThreshold = StartingCritterReproductionThreshold * 2; 

        // Other critter settings
        public const float CritterBirthEnergyShareRatio = 0.3f; //A critter share this percentage of energy with its baby when giving birth
        public const float CritterHungerEnergyThreshold = CritterStartingEnergy * 0.725f; // if energy is below this percentage of starting energy, critter will prioritize finding food 
        public const float CritterSpeedRatioWhenNotHungry = 0.655f; // if the critter is not hungry, it will move at this percentage of its speed to save energy
        public const float CritterMutationRate = 0.155f; // when giving birth, the baby's genes will mutate by this percentage of the parent's genes, in either direction (ex: if mutation rate is 0.1 and parent speed is 100, baby's speed will be between 90 and 110)
        public const float CritterStartingEnergy = 2050f;

        // PREDATOR SETTINGS //
        // Starting values 
        public const float StartingPredatorSightRadius = 37f;
        public const float StartingPredatorSpeed = 190f;
        public const float StartingPredatorMetabolismEfficiency = 0.001f; 
        public const float StartingPredatorReproductionThreshold = PredatorStartingEnergy * 1.375f;
        
        // Max gene values
        public const float MaxPredatorSpeed = StartingPredatorSpeed * 3;
        public const float MaxPredatorSightradius = StartingPredatorSightRadius * 3;
        public const float MaxPredatorMetabolismEfficiency = StartingPredatorMetabolismEfficiency / 3; 
        public const float MaxPredatorReproductionThreshold = StartingPredatorReproductionThreshold / 3; 
        
        // Min gene values
        public const float MinPredatorSpeed = StartingPredatorSpeed / 1.5f;
        public const float MinPredatorSightradius = StartingPredatorSightRadius / 1.5f;
        public const float MinPredatorMetabolismEfficiency = StartingPredatorMetabolismEfficiency * 1.5f; 
        public const float MinPredatorReproductionThreshold = StartingPredatorReproductionThreshold * 1.5f; 

        // Other predator settings
        public const float PredatorBirthEnergyShareRatio = 0.675f; //A predator share this percentage of energy with its baby when giving birth
        public const float PredatorHungerEnergyThreshold = PredatorStartingEnergy * 0.655f; // if energy is below this percentage of starting energy, predator will prioritize finding food 
        public const float PredatorSpeedRatioWhenNotHungry = 0.6525f; // if the predator is not hungry, it will move at this percentage of its speed to save energy
        public const float PredatorMutationRate = 0.375f; // when giving birth, the baby's genes will mutate by this percentage of the parent's genes, in either direction (ex: if mutation rate is 0.35 and parent speed is 500, baby's speed will be between 325 and 675)
        public const float PredatorEnergyGainFromConsumption =  0.475f;  // when a predator eats any other animal,  it gains this percentage of its current energy
        public const float PredatorStartingEnergy = 1250f;
        public const float PredatorCannibalThreshold = PredatorStartingEnergy * 0.475f; // when predator energy is below this threshold, it will consider cannibalism as a food source
        public const float PredatorCannibalSightRadiusBuff = 1.25f; // when in cannibal mode, predator's sight radius is multiplied by this value to help find other predators to eat

        // SMARTY SETTINGS // going to make these avergae compared to the other 2, must come back to review them
        public const float SmartyStartingEnergy = 1250f;
        public const float StartingSmartySightRadius = 40f;
        public const float StartingSmartySpeed = 80f;
        public const float StartingSmartyMetabolismEfficiency = 0.00045f; 
        public const float StartingSmartyReproductionThreshold = SmartyStartingEnergy * 1.5f; // this is not based on energy like the other entities, since the smarty will have a different reproduction mechanism based on its unique behavior, so for simplicity we will just base it on speed, which is a key factor in the smarty genome and will create interesting dynamics as the smarty evolves, since faster smarties will be able to reproduce more quickly, but they will also have higher energy requirements to sustain their speed, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
        public const float SmartyBirthEnergyShareRatio = 0.755f; // A smarty shares this percentage of energy with its baby when giving birth, this is higher than the other entities since the smarty has a more complex behavior and will likely require more energy to sustain itself and its offspring, this also creates interesting dynamics as the smarty evolves, since it will need to balance the energy it shares with its offspring with the energy it needs to sustain itself, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism

        // Max gene values
        public const float MaxSmartySpeed = StartingSmartySpeed * 1.5f;
        public const float MaxSmartySightRadius = StartingSmartySightRadius * 1.5f;
        public const float MaxSmartyMetabolismEfficiency = StartingSmartyMetabolismEfficiency / 1.5f; 
        public const float MaxSmartyReproductionThreshold = StartingSmartyReproductionThreshold / 1.5f; 
        
        // Min gene values
        public const float MinSmartySpeed = StartingSmartySpeed / 3f;
        public const float MinSmartySightRadius = StartingSmartySightRadius / 3f;
        public const float MinSmartyMetabolismEfficiency = StartingSmartyMetabolismEfficiency * 3f; 
        public const float MinSmartyReproductionThreshold = StartingSmartyReproductionThreshold * 3f;
        // other settings
       
        public const float SmartyHungerEnergyThreshold = StartingSmartySpeed * 0.8125f; // if energy is below this threshold, smarty will prioritize finding food, this is not based on a percentage of starting energy like the other entities since the smarty has a different reproduction mechanism and behavior, so for simplicity we will just base it on speed, which is a key factor in the smarty genome and will create interesting dynamics as the smarty evolves, since faster smarties will have higher hunger thresholds and will need to find food more frequently to sustain themselves, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
        public const float SmartySpeedRatioWhenNotHungry = 0.9f; // if the smarty is not hungry, it will move at this percentage of its speed to save energy, this is higher than the other entities since the smarty has a more complex behavior and will likely require more energy to sustain itself, this also creates interesting dynamics as the smarty evolves, since it will need to balance the energy it saves by moving slower when not hungry with the energy it needs to sustain itself and its offspring, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
        public const float SmartyMutationRate = 0.175f; // when giving birth, the baby's genes will mutate by this percentage of the parent's genes, in either direction (ex: if mutation rate is 0.205 and parent speed is 135, baby's speed will be between 107.325 and 162.675), this is higher than the other entities since the smarty has a more complex behavior and will likely require more energy to sustain itself and its offspring, this also creates interesting dynamics as the smarty evolves, since it will need to balance the energy it invests in its offspring with the energy it needs to sustain itself, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
        public const float StartingSmartyPathUpdateInterval = 0.5f; // interval at which a new scan is performed to update the smarties' paths to their targets, this is important to ensure that the smarties are able to adapt to changes in the environment and the movement of their targets, without updating their paths too frequently which can cause performance issues, this also allows us to see how the smarties evolve their pathfinding behavior over time, since they will need to balance the energy they invest in finding optimal paths with the energy they need to sustain themselves and reproduce, which creates a trade-off that the smarties will need to navigate as they evolve, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
        public const float SmartyMinimumPathUpdateInterval = 0.75f; // the minimum interval between path updates for the smarties, this is important to prevent the smarties from updating their paths too frequently, which can cause performance issues, especially as the population grows and there are more entities to process each frame, this also creates interesting dynamics as the smarty evolves, since it will need to balance the energy it invests in finding optimal paths with the energy it needs to sustain itself and reproduce, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
        public const float SmartyPathUpdateIntervalReductionFactor = 0.925f; // when a smarty updates its path, the next update interval is multiplied by this factor, which creates a dynamic where smarties that frequently update their paths will do so more and more frequently, which can be beneficial for smarties that are in complex environments with many obstacles and predators, but can also create a risk of performance issues if the interval gets too low, this also creates interesting dynamics as the smarty evolves, since it will need to balance the energy it invests in finding optimal paths with the energy it needs to sustain itself and reproduce, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
        public const float SmartyPathfindingSightRadiusMultiplier = 1.75f; // the smarty will have a larger sight radius when pathfinding to help it find paths around obstacles and navigate the environment more effectively, this is important to ensure that the smarties are able to find their way to their targets even in complex environments with many obstacles, this also creates interesting dynamics as the smarty evolves, since it will need to balance the energy it invests in pathfinding with the energy it needs to sustain itself and reproduce, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
        public const float SmartyPathfindingSpeedReductionMultiplier = 0.005f; // the smarty will move slower when pathfinding to help it navigate more carefully and avoid obstacles, this is important to ensure that the smarties are able to find their way to their targets even in complex environments with many obstacles, this also creates interesting dynamics as the smarty evolves, since it will need to balance the energy it invests in pathfinding with the energy it needs to sustain itself and reproduce, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
        public const float SmartyScanDuration = 0.755f; // the duration of the smarty scan, during which the smarty will be stationary and scanning its surroundings to update its path to its target, this is important to ensure that the smarties are able to find their way to their targets even in complex environments with many obstacles, without making the scan so long that it leaves the smarty vulnerable to predators or starvation, this also creates interesting dynamics as the smarty evolves, since it will need to balance the energy it invests in scanning with the energy it needs to sustain itself and reproduce, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
        public const float SmartyAmbitionPulseTime = SmartyScanDuration * 0.45f; // the time at which the smarty will perform its ambition pulse during the scan, this is when the smarty will determine its best destination based on its surroundings and its target, this is important to ensure that the smarties are able to find their way to their targets even in complex environments with many obstacles, without making the ambition pulse so early that it doesn't have enough information about its surroundings, or so late that it leaves the smarty vulnerable to predators or starvation, this also creates interesting dynamics as the smarty evolves, since it will need to balance the energy it invests in finding optimal paths with the energy it needs to sustain itself and reproduce, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism
        public const float SmartySafetyPulseTime = SmartyScanDuration * 0.9f; // the time at which the smarty will perform its safety pulse during the scan, this is when the smarty will check if there are any predators near its current path and adjust its path if necessary to avoid them, this is important to ensure that the smarties are able to find their way to their targets even in complex environments with many obstacles and predators, without making the safety pulse so early that it doesn't have enough information about its surroundings, or so late that it leaves the smarty vulnerable to predators or starvation, this also creates interesting dynamics as the smarty evolves, since it will need to balance the energy it invests in finding optimal paths and avoiding predators with the energy it needs to sustain itself and reproduce, which creates a trade-off that the smarty will need to navigate as it evolves, this also allows us to see how the smarty evolves differently from the other entities in the ecosystem, since it has a unique set of challenges and advantages based on its behavior and reproduction mechanism

        // FOODPELLET SETTINGS //
        public const float FoodPelletRateOfReproduction = 2f; // seconds
        public const int FoodPelletMaxNumberPerRegion = 80; // if the number of pellets around it exceeds this number, no more reproduction will occur
        public const float FoodPelletEnergyValue = 150f;
        public const float FoodPelletSpreadAmount = 150f; // the maximum distance from the parent pellet that a new pellet can spawn, this creates a more natural spread of food pellets across the world instead of them all clumping up in the same spot

        // Utility functions
        /// <summary>
        /// Snaps a position to be within the legal world boundaries.
        /// </summary>
        public static Vector2 GetLegalPosition(Vector2 pos)
        {
            float x = Math.Clamp(pos.X, 0, WorldWidth);
            float y = Math.Clamp(pos.Y, 0, WorldHeight);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Ensures that all required save files exist, creating them if they don't.
        /// </summary>
        public static void EnsureFilesExist()
        {
            // Create an array of the absolute paths we defined above, easy to scale if we add more files in the future
            string[] allFiles = {
                CSVStatsFile,
                WorldSaveFile,
                PopulationHTMLFile,
                CritterDataHTMLFile,
                PredatorDataHTMLFile
            };

            foreach (string filePath in allFiles)
            {
                // Getting the directory name from the full path
                string directory = Path.GetDirectoryName(filePath);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    // This creates the entire folder chain (Exports/Subfolders) at once
                    Directory.CreateDirectory(directory);
                }
            }
        }
    }
}