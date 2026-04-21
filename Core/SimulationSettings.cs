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

        // Simulation Balance
        public const float StartingEnergy = 2000f;
        public const float EatDistance = 10f;
        public const float BaseMetabolism = 1.0f;
        //public const float MovementMultiplier = 0.005f; //rate of metabolism goes up when moving

        // Initialization settings
        public const int InitialCritterNumber = 50;
        public const int InitialFoodPelletNumber = 200;

        //Entities Settings
        //Critters
        public const float StartingCritterSightRadius = 60f;
        public const float StartingCritterSpeed = 150f;
        public const float StartingCritterMetabolismEfficiency = 0.0035f; // might need to adjust this later
        public const float StartingCritterReproductionThreshold = StartingEnergy * 1.3f;
        public const float MaxCritterSpeed = StartingCritterSpeed * 2;
        public const float MaxCritterSightradius = StartingCritterSightRadius * 2;
        public const float MaxCritterMetabolismEfficiency = StartingCritterMetabolismEfficiency / 2; // might need to adjust this later
        public const float MaxCritterReproductionThreshold = StartingCritterReproductionThreshold / 2; //might need to adjust this later



        //FoodPellet
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