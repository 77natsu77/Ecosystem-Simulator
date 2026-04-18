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
        public float EnergyValue => Settings.FoodPelletEnergyValue;

        private double _age = 0;
        public event SpawnRequestDelegate OnSpawnRequested;

        public FoodPellet(Vector2 SpawnPoint)
        {
            this.Position = SpawnPoint;
        }

        public void Update(double deltaTime, IEnumerable<IEntity> nearby)
        {
            _age += deltaTime;

            // Reproduce if old enough and not too crowded
            if (_age > Settings.FoodPelletRateOfReproduction)
            {
                _age = 0;
                int nearbyCount = 0;
                foreach (var e in nearby) { if (e is IEatable) nearbyCount++; }

                if (nearbyCount < Settings.FoodPelletMaxNumberPerRegion)
                {
                    Vector2 spawnPos = this.Position;
                    bool isValid = false;
                    int attempts = 0;

                    // Loop until we find a spot or hit a safety limit (10 tries)
                    while (!isValid && attempts < 10)
                    {
                        float offsetX = (float)(Settings.Rng.NextDouble() * 40 - 20);
                        float offsetY = (float)(Settings.Rng.NextDouble() * 40 - 20);
                        spawnPos = new Vector2(this.Position.X + offsetX, this.Position.Y + offsetY);

                        // The Check: Is it inside the world?
                        if (spawnPos.X >= 0 && spawnPos.X <= Settings.WorldWidth &&
                            spawnPos.Y >= 0 && spawnPos.Y <= Settings.WorldHeight)
                        {
                            isValid = true;
                        }
                        attempts++;
                    }

                    if (isValid)
                    {
                        //Trigger spawn event
                        OnSpawnRequested?.Invoke(new FoodPellet(spawnPos));
                    }
                }
            }
        }

        public void Consume() => IsPendingRemoval = true;
    }
}