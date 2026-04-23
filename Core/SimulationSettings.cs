using System;

namespace Ecosystem_Simulator.Core
{
    public static class Settings
    {
        // RNG
        public static readonly Random Rng = new Random();

        // World Dimensions
        public const float WorldWidth = 1200f;
        public const float WorldHeight = 800f;

        // Grid Settings
        public const float CellSize = 100;
        //Save files
        public const string WorldSaveFile = "../Exports/WorldData.json"; //TODO CREATE A FUNCTION GOING THROUGH EACH OF THESE FILES AND ENSURING THEY EXIST, RATHER THAN DOING IT IN STATS MANAGER
        public const string StatsFilePath = "../Exports/stats.csv";
        public const string PopulationHTMLFile = "../Exports/population_over_time.html"; //TODO: MAKE THE FILES ACTUALLY APPEAR IN THE EXPORTS FOLDER
        public const string CritterDataHTMLFile = "../Exports/critter_data_over_time.html";
        //TODO ADD THE COLORS USED TO REPRESENT DATA TO SETTINGS FILE
        //Save settings
        public const int StatsSaveRate = 3; // seconds
        //UI SETTINGS
        public const double TickRate = 0.0166666666666667; //around 60 FPS

        // Simulation Balance
        public const float StartingEnergy = 2000f;
        public const float EatDistance = 10f;
        public const float BaseMetabolism = 1.0f;
<<<<<<< HEAD
        public const float MutationRate = 0.1; // 10%
=======
        public const float MutationRate = 3f; // %, might need to make different mutation rates for different entities when the time comes
>>>>>>> 1c9d56c (Worked on visualizing statistics and drafting plans for final version)
        //public const float MovementMultiplier = 0.005f; //rate of metabolism goes up when moving

        // Initialization settings
        public const int InitialCritterNumber = 50;
        public const int InitialFoodPelletNumber = 200;

        //ENTITIES SETTINGS
        //Critters
        //Starting values
        public const float StartingCritterSightRadius = 60f;
        public const float StartingCritterSpeed = 150f;
        public const float StartingCritterMetabolismEfficiency = 0.0035f; // might need to adjust this later
<<<<<<< HEAD
        public const float StartingCritterReproductionThreshold = StartingEnergy * 1.3f;
=======
        public const float StartingCritterReproductionThreshold = StartingEnergy * 1.2f;
>>>>>>> 1c9d56c (Worked on visualizing statistics and drafting plans for final version)
        //Max values
        public const float MaxCritterSpeed = StartingCritterSpeed * 2;
        public const float MaxCritterSightradius = StartingCritterSightRadius * 2;
        public const float MaxCritterMetabolismEfficiency = StartingCritterMetabolismEfficiency / 2; // might need to adjust this later
        public const float MaxCritterReproductionThreshold = StartingCritterReproductionThreshold / 2; //might need to adjust this later
        //Min values
        public const float MinCritterSpeed = StartingCritterSpeed / 2;
        public const float MinCritterSightradius = StartingCritterSightRadius / 2;
        public const float MinCritterMetabolismEfficiency = StartingCritterMetabolismEfficiency * 2; // might need to adjust this later
        public const float MinCritterReproductionThreshold = StartingCritterReproductionThreshold * 2; //might need to adjust this later
<<<<<<< HEAD
=======
        //Other criiter settings
        public const float CritterBirthEnergyShareRatio = 0.15f; //A critter share this percentage of energy with its baby when giving birth
>>>>>>> 1c9d56c (Worked on visualizing statistics and drafting plans for final version)



        //FoodPellet settings
        public const float FoodPelletRateOfReproduction = 8.5f; // seconds
        public const int FoodPelletMaxNumberPerRegion = 20; // if the number of pellets around it exceeds this number, no more reproduction will occur
        public const float FoodPelletEnergyValue = 100f;

        //Utility functions
        /// <summary>
        /// Snaps a position to be within the legal world boundaries.
        /// </summary>
        public static Vector2 GetLegalPosition(Vector2 pos)
        {
            float x = Math.Max(0, Math.Min(WorldWidth, pos.X));
            float y = Math.Max(0, Math.Min(WorldHeight, pos.Y));
            return new Vector2(x, y);
        }
    }
}