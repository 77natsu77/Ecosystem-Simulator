using System;

namespace Ecosystem_Simulator.Core
{
    public static class Settings
    {

        public static readonly Random Rng = new Random();

        // World Dimensions
        public const float WorldWidth = 1200f;
        public const float WorldHeight = 800f;

        // Grid Settings
        public const float CellSize = 100;

        // Simulation Balance
        public const float StartingEnergy = 100f;
        public const float EatDistance = 5.0f;

        //Entities Settings
        public const float SightRadius = 50f;
        public const float CritterSpeed = 150f;
    }
}