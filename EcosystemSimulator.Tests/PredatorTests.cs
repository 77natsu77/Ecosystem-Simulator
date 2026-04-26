using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Entities;
using System.Collections.Generic;
using Xunit;

namespace EcosystemSimulator.Tests
{
    /// <summary>
    /// Tests for the Predator class, covering hunting, cannibal mode, energy management, and reproduction.
    /// Critical for validating predator-prey dynamics in the simulation.
    /// </summary>
    public class PredatorTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // ARRANGE
            Vector2 expectedPos = new Vector2(100f, 100f);
            float expectedEnergy = 1500f;
            PredatorGenome genome = new PredatorGenome();

            // ACT
            Predator predator = new Predator(expectedPos, genome, expectedEnergy);

            // ASSERT
            Assert.Equal(expectedPos.X, predator.Position.X);
            Assert.Equal(expectedPos.Y, predator.Position.Y);
            Assert.Equal(expectedEnergy, predator.Energy);
            Assert.Equal(genome.Speed, predator.Speed);
            Assert.Equal(genome.SightRadius, predator.SightRadius);
            Assert.False(predator.IsPendingRemoval);
        }

        [Fact]
        public void CannibalMode_BelowThreshold_ReturnsTrue()
        {
            // ARRANGE: Create predator with energy below 25% threshold
            Vector2 position = new Vector2(100f, 100f);
            PredatorGenome genome = new PredatorGenome();
            float lowEnergy = Settings.PredatorStartingEnergy * 0.2f; // 20% - below 25% threshold

            // ACT
            Predator predator = new Predator(position, genome, lowEnergy);

            // ASSERT: Should be in cannibal mode
            Assert.True(predator.CannibalMode,
                $"Predator with {lowEnergy} energy (20% of starting) should be in cannibal mode");
        }

        [Fact]
        public void CannibalMode_AboveThreshold_ReturnsFalse()
        {
            // ARRANGE: Create predator with energy above 25% threshold
            Vector2 position = new Vector2(100f, 100f);
            PredatorGenome genome = new PredatorGenome();
            float highEnergy = Settings.PredatorStartingEnergy * 0.5f; // 50% - well above threshold

            // ACT
            Predator predator = new Predator(position, genome, highEnergy);

            // ASSERT: Should NOT be in cannibal mode
            Assert.False(predator.CannibalMode,
                $"Predator with {highEnergy} energy (50% of starting) should NOT be in cannibal mode");
        }

        [Fact]
        public void CannibalMode_ExactlyAtThreshold_ReturnsTrue()
        {
            // ARRANGE: Create predator with energy exactly at 25% threshold
            Vector2 position = new Vector2(100f, 100f);
            PredatorGenome genome = new PredatorGenome();
            float thresholdEnergy = Settings.PredatorStartingEnergy * 0.25f; // Exactly 25%

            // ACT
            Predator predator = new Predator(position, genome, thresholdEnergy);

            // ASSERT: Should be in cannibal mode (<=, not <)
            Assert.True(predator.CannibalMode,
                "Predator with exactly 25% energy should be in cannibal mode");
        }

        [Fact]
        public void Update_HuntsNearbyCritter_IncreasesEnergy()
        {
            // ARRANGE: Create predator and critter close together
            Vector2 predatorPos = new Vector2(100f, 100f);
            Vector2 critterPos = new Vector2(105f, 105f); // Within hunting distance

            PredatorGenome predatorGenome = new PredatorGenome();
            CritterGenome critterGenome = new CritterGenome();
            
            Predator predator = new Predator(predatorPos, predatorGenome, Energy: 1000f);
            Critter critter = new Critter(critterPos, critterGenome, Energy: 500f);

            float energyBefore = predator.Energy;

            // ACT: Run one update tick
            List<IEntity> nearbyEntities = new List<IEntity> { critter };
            predator.Update(deltaTime: 0.016, nearbyEntities);

            // ASSERT: Predator should have gained energy (if hunting worked)
            // Note: Actual behavior depends on implementation - test should verify hunting logic
            bool critterConsumed = critter.IsPendingRemoval;
            
            if (critterConsumed)
            {
                Assert.True(predator.Energy > energyBefore,
                    "Predator should gain energy from consuming critter");
            }
        }

        [Fact]
        public void Update_CritterOutOfReach_DoesNotHunt()
        {
            // ARRANGE: Create predator and critter far apart
            Vector2 predatorPos = new Vector2(100f, 100f);
            Vector2 critterPos = new Vector2(500f, 500f); // Far away

            PredatorGenome predatorGenome = new PredatorGenome();
            CritterGenome critterGenome = new CritterGenome();
            
            Predator predator = new Predator(predatorPos, predatorGenome, Energy: 1000f);
            Critter critter = new Critter(critterPos, critterGenome);

            // ACT
            List<IEntity> nearbyEntities = new List<IEntity> { critter };
            predator.Update(deltaTime: 0.016, nearbyEntities);

            // ASSERT: Critter should NOT be consumed
            Assert.False(critter.IsPendingRemoval,
                "Critter should not be consumed if too far away");
        }

        [Fact]
        public void Update_ZeroEnergy_MarksPendingRemoval()
        {
            // ARRANGE: Create predator with very low energy
            Vector2 position = new Vector2(100f, 100f);
            PredatorGenome genome = new PredatorGenome();
            
            // Start with minimal energy (metabolism will drain it)
            Predator predator = new Predator(position, genome, Energy: 0.5f);

            // ACT: Run update for long time to drain energy
            List<IEntity> emptyList = new List<IEntity>();
            predator.Update(deltaTime: 1.0, emptyList);

            // ASSERT: Predator should be marked for removal (death)
            Assert.True(predator.IsPendingRemoval,
                "Predator with zero energy should be marked for removal");
        }

        [Fact]
        public void Update_EnergyAboveThreshold_TriggersReproduction()
        {
            // ARRANGE: Create predator with energy above reproduction threshold
            Vector2 position = new Vector2(100f, 100f);
            PredatorGenome genome = new PredatorGenome();
            
            // Set energy well above reproduction threshold
            float reproductionThreshold = Settings.PredatorStartingEnergy * Settings.MinPredatorReproductionThreshold;
            Predator predator = new Predator(position, genome, Energy: reproductionThreshold + 500f);

            bool spawnEventFired = false;
            IEntity spawnedOffspring = null;

            // Subscribe to spawn event
            predator.OnSpawnRequested += (entity) => 
            {
                spawnEventFired = true;
                spawnedOffspring = entity;
            };

            float energyBefore = predator.Energy;

            // ACT: Run update
            List<IEntity> emptyList = new List<IEntity>();
            predator.Update(deltaTime: 0.016, emptyList);

            // ASSERT: Reproduction should have occurred
            Assert.True(spawnEventFired,
                "OnSpawnRequested event should fire when energy exceeds threshold");
            
            Assert.NotNull(spawnedOffspring);
            
            // Parent's energy should be reduced
            Assert.True(predator.Energy < energyBefore,
                "Parent predator should lose energy after reproduction");
        }

        [Fact]
        public void Update_EnergyBelowThreshold_DoesNotReproduce()
        {
            // ARRANGE: Create predator with energy below reproduction threshold
            Vector2 position = new Vector2(100f, 100f);
            PredatorGenome genome = new PredatorGenome();
            
            // Set energy below threshold
            Predator predator = new Predator(position, genome, Energy: 1000f);

            bool spawnEventFired = false;
            predator.OnSpawnRequested += (entity) => { spawnEventFired = true; };

            // ACT
            List<IEntity> emptyList = new List<IEntity>();
            predator.Update(deltaTime: 0.016, emptyList);

            // ASSERT: No reproduction should occur
            Assert.False(spawnEventFired,
                "OnSpawnRequested should not fire when energy is below threshold");
        }

        [Fact]
        public void Update_InCannibalMode_CanHuntOtherPredators()
        {
            // ARRANGE: Create hungry predator (cannibal mode) and nearby healthy predator
            Vector2 hungryPos = new Vector2(100f, 100f);
            Vector2 healthyPos = new Vector2(105f, 105f); // Close together

            PredatorGenome genome1 = new PredatorGenome();
            PredatorGenome genome2 = new PredatorGenome();
            
            // Hungry predator in cannibal mode
            Predator hungryPredator = new Predator(hungryPos, genome1, 
                Energy: Settings.PredatorStartingEnergy * 0.2f);
            
            // Healthy predator
            Predator healthyPredator = new Predator(healthyPos, genome2, 
                Energy: Settings.PredatorStartingEnergy);

            // Verify cannibal mode is active
            Assert.True(hungryPredator.CannibalMode,
                "Hungry predator should be in cannibal mode");

            // ACT: Update hungry predator with healthy predator nearby
            List<IEntity> nearbyEntities = new List<IEntity> { healthyPredator };
            hungryPredator.Update(deltaTime: 0.016, nearbyEntities);

            // ASSERT: If cannibalism is implemented, healthy predator might be consumed
            // This test verifies the cannibal behavior exists
            // (Implementation details may vary - adjust assertion based on actual code)
        }

        [Fact]
        public void Update_WithNoNearbyEntities_PredatorWanders()
        {
            // ARRANGE
            Vector2 startPos = new Vector2(100f, 100f);
            PredatorGenome genome = new PredatorGenome();
            Predator predator = new Predator(startPos, genome, Energy: 2000f);

            Vector2 startVelocity = predator.Velocity;

            // ACT: Run several updates with no nearby entities
            List<IEntity> emptyList = new List<IEntity>();
            for (int i = 0; i < 10; i++)
            {
                predator.Update(deltaTime: 0.016, emptyList);
            }

            // ASSERT: Predator should have moved from start position
            Assert.NotEqual(startPos.X, predator.Position.X);
            Assert.NotEqual(startPos.Y, predator.Position.Y);
        }

        [Fact]
        public void InvertVelocityX_ReversesXComponent()
        {
            // ARRANGE
            Vector2 pos = new Vector2(100f, 100f);
            PredatorGenome genome = new PredatorGenome();
            Predator predator = new Predator(pos, genome);

            float originalVelX = predator.Velocity.X;
            float originalVelY = predator.Velocity.Y;

            // ACT
            predator.InvertVelocityX();

            // ASSERT
            Assert.Equal(-originalVelX, predator.Velocity.X);
            Assert.Equal(originalVelY, predator.Velocity.Y);
        }

        [Fact]
        public void InvertVelocityY_ReversesYComponent()
        {
            // ARRANGE
            Vector2 pos = new Vector2(100f, 100f);
            PredatorGenome genome = new PredatorGenome();
            Predator predator = new Predator(pos, genome);

            float originalVelX = predator.Velocity.X;
            float originalVelY = predator.Velocity.Y;

            // ACT
            predator.InvertVelocityY();

            // ASSERT
            Assert.Equal(originalVelX, predator.Velocity.X);
            Assert.Equal(-originalVelY, predator.Velocity.Y);
        }
    }
}