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
        public const float MovementMultiplier = 0.015f; //rate of metabolism goes up when moving

        // Initialization settings
        public const int InitialCritterNumber = 10;
        public const int InitialFoodPelletNumber = 200;

        //Entities Settings
        //Critters
        public const float CritterSightRadius = 60f;
        public const float StartingCritterSpeed = 150f;
        public const float MaxCritterSpeed = StartingCritterSpeed * 2;
        public const float CritterReproductionThreshold = StartingEnergy * 1.3f;

        //FoodPellet
        public const float FoodPelletRateOfReproduction = 5; //seconds
        public const int FoodPelletMaxNumberPerRegion = 20; //if the number of pellets around it exceeds this number, no more reproduction will occur
    }
}