using System;
using System.IO;

namespace Ecosystem_Simulator.Core
{
    public static class Settings //TODO: ENSURE ALL SETTINGS ARE ORGANISED IN AN APPROPRIATE MANNER
    {
        // RNG
        public static readonly Random Rng = new Random();

        // WORLD SETTINGS
        public const float WorldWidth = 1100f;
        public const float WorldHeight = 700f;

        // GRID SETTINGS
        public const float CellSize = 100f;
        // Large prime numbers for hashing
        public const int HashConstantX = 73856093; 
        public const int HashConstantY = 19349663; 
        
        // SAVE SETTINGS
        // file(s) settings
        private static readonly string RootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
    
        private static readonly string ExportsFolder = Path.Combine(RootPath, "Exports");

        public static readonly string WorldSaveFile = Path.Combine(ExportsFolder, "WorldData.json"); 
        public static readonly string StatsFilePath = Path.Combine(ExportsFolder, "stats.csv");
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
        public const int StatsSaveRate = 1; // seconds, the rate at which data is saved to files(this does not include world data which is saved at the very end)
        
        //UI SETTINGS
        public const double TickRate = 0.0166666666666667; //around 60 FPS
        public const bool DISPLAY_VELOCITY_ARROWS = true;

        // SIMULATION BALANCE SETTINGS
        public const float CritterEatDistance = 10f;
        public const float PredatorEatDistance = 10f;
        public const float BaseMetabolism = 1.5f;



        

        // INITIALIZATION SETTINGS
        public const int InitialCritterNumber = 20;
        public const int InitialPredatorNumber = 5;
        public const int InitialFoodPelletNumber = 100;
        

        // ENTITIES SETTINGS
        // CRITTER SETTINGS //
        // Starting values
        public const float StartingCritterSightRadius = 50f;
        public const float StartingCritterSpeed = 120f;
        public const float StartingCritterMetabolismEfficiency = 0.0055f; 
        public const float StartingCritterReproductionThreshold = CritterStartingEnergy * 1.8f;
        

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
        public const float CritterHungerEnergy = CritterStartingEnergy * 0.7f; // if energy is below this percentage of starting energy, critter will prioritize finding food 
        public const float CritterSpeedRatioWhenNotHungry = 0.65f; // if the critter is not hungry, it will move at this percentage of its speed to save energy
        public const float CritterMutationRate = 0.15f; // when giving birth, the baby's genes will mutate by this percentage of the parent's genes, in either direction (ex: if mutation rate is 0.1 and parent speed is 100, baby's speed will be between 90 and 110)
        public const float CritterStartingEnergy = 2000f;

        // PREDATOR SETTINGS //
        // Starting values 
        public const float StartingPredatorSightRadius = 15f;
        public const float StartingPredatorSpeed = 90f;
        public const float StartingPredatorMetabolismEfficiency = 0.03f; 
        public const float StartingPredatorReproductionThreshold = PredatorStartingEnergy * 3f;
        

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
        public const float PredatorBirthEnergyShareRatio = 0.655f; //A predator share this percentage of energy with its baby when giving birth
        public const float PredatorHungerEnergy = PredatorStartingEnergy * 0.525f; // if energy is below this percentage of starting energy, predator will prioritize finding food 
        public const float PredatorSpeedRatioWhenNotHungry = 0.85f; // if the predator is not hungry, it will move at this percentage of its speed to save energy
        public const float PredatorMutationRate = 0.375f; // when giving birth, the baby's genes will mutate by this percentage of the parent's genes, in either direction (ex: if mutation rate is 0.35 and parent speed is 500, baby's speed will be between 325 and 675)
        public const float PredatorEnergyGainFromCritter = CritterStartingEnergy * 0.15f; // a 10% energy transfer would be more realistic, but who cares!
        // TODO: implement varying energy gained from cannibalism based on the prey's energy, currently its just a flat value which is not very realistic but it makes the simulation more stable and less likely to have crazy energy spikes from cannibalism, which can cause a lot of chaos in the ecosystem. Maybe in the future I could implement a more complex energy transfer system that takes into account the prey's energy and the predator's metabolism efficiency or something like that.
        public const float PredatorEnergyGainFromPredator = PredatorStartingEnergy * 0.5f;// energy gained from cannibalism...
        public const float PredatorStartingEnergy = 1000f;

        // FOODPELLET SETTINGS //
        public const float FoodPelletRateOfReproduction = 0.55f; // seconds
        public const int FoodPelletMaxNumberPerRegion = 30; // if the number of pellets around it exceeds this number, no more reproduction will occur
        public const float FoodPelletEnergyValue = 140f;

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
                StatsFilePath,
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