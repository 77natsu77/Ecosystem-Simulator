using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.delegates;
using Ecosystem_Simulator.Environment;
using System.Collections.Generic;

namespace Ecosystem_Simulator.Entities
{
    public class FoodPellet : IEatable, IUpdatable
    {
        public Vector2 Position { get; private set; }
        public bool IsPendingRemoval { get; private set; }
        public float EnergyValue => 25f;

        private double _age = 0;
        private const double MaxAge = 30.0; // Seconds before it rots
        public event SpawnRequestDelegate OnSpawnRequested;

        public FoodPellet(Vector2 SpawnPoint)
        {
            this.Position = SpawnPoint;
        }

        public void Update(double deltaTime, IEnumerable<IEntity> nearby)
        {
            _age += deltaTime;

            // Reproduce if old enough and not too crowded
            if (_age > 10.0)
            {
                _age = 0;
                // Simple "Seed" logic: spawn a new pellet nearby
                int nearbyCount = 0;
                foreach (var e in nearby) { if (e is IEatable) nearbyCount++; }
                if (nearbyCount < 5) // Only spawn if the "neighborhood" isn't full
                {
                    //calculate spawn position
                    float offsetX = (float)(Settings.Rng.NextDouble() * 40 - 20);
                    float offsetY = (float)(Settings.Rng.NextDouble() * 40 - 20);

                    Vector2 spawnPos = new Vector2(this.Position.X + offsetX, this.Position.Y + offsetY);

                    // Trigger the spawn event
                    OnSpawnRequested?.Invoke(new FoodPellet(spawnPos));
                }
                
            }
        }

        public void Consume() => IsPendingRemoval = true;
    }
}