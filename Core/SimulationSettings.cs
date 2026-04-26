using System;
using System.IO;

namespace Ecosystem_Simulator.Core
{
    public static class Settings //TODO: ENSURE ALL SETTINGS ARE ORGANISED IN AN APPROPRIATE MANNER
    {
        // RNG
        public static readonly Random Rng = new Random();

        // WORLD SETTINGS
        public const float WorldWidth = 1200f;
        public const float WorldHeight = 1000f;

        // GRID SETTINGS
        public const float CellSize = 100f;
        // Large prime numbers for hashing
        public const int HashConstantX = 73856093; 
        public const int HashConstantY = 19349663; 
        
        // SAVE SETTINGS
        // file(s) settings
        public const string WorldSaveFile = "../Exports/WorldData.json"; 
        public const string StatsFilePath = "../Exports/stats.csv";
        public const string PopulationHTMLFile = "../Exports/population_over_time.html"; 
        public const string CritterDataHTMLFile = "../Exports/critter_data_over_time.html";
        public const string PredatorDataHTMLFile = "../Exports/predator_data_over_time.html";

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
        public const float CritterEatDistance = 20f;
        public const float PredatorEatDistance = 10f;
        public const float BaseMetabolism = 1.0f;



        

        // INITIALIZATION SETTINGS
        public const int InitialCritterNumber = 50;
        public const int InitialPredatorNumber = 5;
        public const int InitialFoodPelletNumber = 500;
        

        // ENTITIES SETTINGS
        // CRITTER SETTINGS //
        // Starting values
        public const float StartingCritterSightRadius = 50f;
        public const float StartingCritterSpeed = 120f;
        public const float StartingCritterMetabolismEfficiency = 0.0055f; 
        public const float StartingCritterReproductionThreshold = CritterStartingEnergy * 1.2f;
        

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
        public const float CritterSpeedRatioWhenNotHungry = 0.8f; // if the critter is not hungry, it will move at this percentage of its speed to save energy
        public const float CritterMutationRate = 0.15f; // when giving birth, the baby's genes will mutate by this percentage of the parent's genes, in either direction (ex: if mutation rate is 0.1 and parent speed is 100, baby's speed will be between 90 and 110)
        public const float CritterStartingEnergy = 2000f;

        // PREDATOR SETTINGS //
        // Starting values 
        public const float StartingPredatorSightRadius = 20f;
        public const float StartingPredatorSpeed = 200f;
        public const float StartingPredatorMetabolismEfficiency = 0.0075f; 
        public const float StartingPredatorReproductionThreshold = PredatorStartingEnergy * 1.65f;
        

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
        public const float PredatorBirthEnergyShareRatio = 0.65f; //A predator share this percentage of energy with its baby when giving birth
        public const float PredatorHungerEnergy = PredatorStartingEnergy * 0.7f; // if energy is below this percentage of starting energy, predator will prioritize finding food 
        public const float PredatorSpeedRatioWhenNotHungry = 0.125f; // if the predator is not hungry, it will move at this percentage of its speed to save energy
        public const float PredatorMutationRate = 0.425f; // when giving birth, the baby's genes will mutate by this percentage of the parent's genes, in either direction (ex: if mutation rate is 0.35 and parent speed is 500, baby's speed will be between 325 and 675)
        public const float PredatorEnergyGainFromCritter = CritterStartingEnergy * 0.3f; // 10% energy transfer would be more realistic, but who cares!
        public const float PredatorEnergyGainFromPredator = PredatorStartingEnergy * 0.5f;// energy gained from cannibalism...
        public const float PredatorStartingEnergy = 1000f;

        // FOODPELLET SETTINGS //
        public const float FoodPelletRateOfReproduction = 1f; // seconds
        public const int FoodPelletMaxNumberPerRegion = 45; // if the number of pellets around it exceeds this number, no more reproduction will occur
        public const float FoodPelletEnergyValue = 120f;

        // Utility functions
        /// <summary>
        /// Snaps a position to be within the legal world boundaries.
        /// </summary>
        public static Vector2 GetLegalPosition(Vector2 pos)
        {
            float x = Math.Max(0, Math.Min(WorldWidth, pos.X));
            float y = Math.Max(0, Math.Min(WorldHeight, pos.Y));
            return new Vector2(x, y);
        }

        /// <summary>
        /// Ensures that all required save files exist, creating them if they don't.
        /// </summary>
        public static void EnsureFilesExist()// could I make this function even better??? maybe add eacheach of the files to a list and iterate through or something similar
        {

            string directory = Path.GetDirectoryName(WorldSaveFile);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            directory = Path.GetDirectoryName(StatsFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            directory = Path.GetDirectoryName(PopulationHTMLFile);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            directory = Path.GetDirectoryName(CritterDataHTMLFile);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            directory = Path.GetDirectoryName(PredatorDataHTMLFile);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}