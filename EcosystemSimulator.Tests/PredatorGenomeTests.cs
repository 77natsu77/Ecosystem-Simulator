using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using System;
using Xunit;

namespace EcosystemSimulator.Tests
{
    /// <summary>
    /// Tests for the PredatorGenome class, focusing on mutation logic and boundary conditions.
    /// Validates that genetic mutations stay within viable ranges for predator evolution.
    /// </summary>
    public class PredatorGenomeTests
    {
        [Fact]
        public void DefaultConstructor_SetsStartingValues()
        {
            // ACT: Create genome with default constructor
            PredatorGenome genome = new PredatorGenome();

            // ASSERT: Should have exact starting values (no mutation)
            Assert.Equal(Settings.StartingPredatorSpeed, genome.Speed);
            Assert.Equal(Settings.StartingPredatorSightRadius, genome.SightRadius);
            Assert.Equal(Settings.StartingPredatorMetabolismEfficiency, genome.MetabolismEfficiency);
            Assert.Equal(Settings.StartingPredatorReproductionThreshold, genome.ReproductionThreshold);
        }

        [Fact]
        public void Mutate_ClampsToBounds_Speed()
        {
            // ARRANGE: Parent with speed at maximum (should mutate but stay clamped)
            float parentSpeed = Settings.MaxPredatorSpeed;
            float parentSight = Settings.StartingPredatorSightRadius;
            float parentMetab = Settings.StartingPredatorMetabolismEfficiency;
            float parentRepro = Settings.StartingPredatorReproductionThreshold;

            // ACT: Create multiple children to test upper bound clamping
            for (int i = 0; i < 20; i++)
            {
                PredatorGenome child = new PredatorGenome(
                    parentSpeed, parentSight, parentMetab, parentRepro
                );

                // ASSERT: Child's speed should never exceed max
                Assert.True(child.Speed <= Settings.MaxPredatorSpeed,
                    $"Speed {child.Speed} exceeds maximum {Settings.MaxPredatorSpeed}");
                
                // Also verify it doesn't go below minimum
                Assert.True(child.Speed >= Settings.MinPredatorSpeed,
                    $"Speed {child.Speed} is below minimum {Settings.MinPredatorSpeed}");
            }
        }

        [Fact]
        public void Mutate_ClampsToBounds_SightRadius()
        {
            // ARRANGE: Test both extremes
            float parentSpeed = Settings.StartingPredatorSpeed;
            float parentSightMin = Settings.MinPredatorSightradius;  // At minimum
            float parentSightMax = Settings.MaxPredatorSightradius;  // At maximum
            float parentMetab = Settings.StartingPredatorMetabolismEfficiency;
            float parentRepro = Settings.StartingPredatorReproductionThreshold;

            // ACT & ASSERT: Test minimum bound
            for (int i = 0; i < 20; i++)
            {
                PredatorGenome childMin = new PredatorGenome(
                    parentSpeed, parentSightMin, parentMetab, parentRepro
                );
                
                Assert.True(childMin.SightRadius >= Settings.MinPredatorSightradius,
                    $"SightRadius {childMin.SightRadius} is below minimum {Settings.MinPredatorSightradius}");
            }

            // ACT & ASSERT: Test maximum bound
            for (int i = 0; i < 20; i++)
            {
                PredatorGenome childMax = new PredatorGenome(
                    parentSpeed, parentSightMax, parentMetab, parentRepro
                );
                
                Assert.True(childMax.SightRadius <= Settings.MaxPredatorSightradius,
                    $"SightRadius {childMax.SightRadius} exceeds maximum {Settings.MaxPredatorSightradius}");
            }
        }

        [Fact]
        public void Mutate_ClampsToBounds_MetabolismEfficiency()
        {
            // ARRANGE
            float parentSpeed = Settings.StartingPredatorSpeed;
            float parentSight = Settings.StartingPredatorSightRadius;
            float parentMetabMin = Settings.MinPredatorMetabolismEfficiency;  // At minimum
            float parentMetabMax = Settings.MaxPredatorMetabolismEfficiency;  // At maximum
            float parentRepro = Settings.StartingPredatorReproductionThreshold;

            // ACT & ASSERT: Test minimum bound
            for (int i = 0; i < 20; i++)
            {
                PredatorGenome childMin = new PredatorGenome(
                    parentSpeed, parentSight, parentMetabMin, parentRepro
                );
                
                Assert.True(childMin.MetabolismEfficiency >= Settings.MinPredatorMetabolismEfficiency,
                    $"MetabolismEfficiency {childMin.MetabolismEfficiency} is below minimum {Settings.MinPredatorMetabolismEfficiency}");
            }

            // ACT & ASSERT: Test maximum bound
            for (int i = 0; i < 20; i++)
            {
                PredatorGenome childMax = new PredatorGenome(
                    parentSpeed, parentSight, parentMetabMax, parentRepro
                );
                
                Assert.True(childMax.MetabolismEfficiency <= Settings.MaxPredatorMetabolismEfficiency,
                    $"MetabolismEfficiency {childMax.MetabolismEfficiency} exceeds maximum {Settings.MaxPredatorMetabolismEfficiency}");
            }
        }

        [Fact]
        public void Mutate_ClampsToBounds_ReproductionThreshold()
        {
            // ARRANGE
            float parentSpeed = Settings.StartingPredatorSpeed;
            float parentSight = Settings.StartingPredatorSightRadius;
            float parentMetab = Settings.StartingPredatorMetabolismEfficiency;
            float parentReproMin = Settings.MinPredatorReproductionThreshold;  // At minimum
            float parentReproMax = Settings.MaxPredatorReproductionThreshold;  // At maximum

            // ACT & ASSERT: Test minimum bound
            for (int i = 0; i < 20; i++)
            {
                PredatorGenome childMin = new PredatorGenome(
                    parentSpeed, parentSight, parentMetab, parentReproMin
                );
                
                Assert.True(childMin.ReproductionThreshold >= Settings.MinPredatorReproductionThreshold,
                    $"ReproductionThreshold {childMin.ReproductionThreshold} is below minimum {Settings.MinPredatorReproductionThreshold}");
            }

            // ACT & ASSERT: Test maximum bound
            for (int i = 0; i < 20; i++)
            {
                PredatorGenome childMax = new PredatorGenome(
                    parentSpeed, parentSight, parentMetab, parentReproMax
                );
                
                Assert.True(childMax.ReproductionThreshold <= Settings.MaxPredatorReproductionThreshold,
                    $"ReproductionThreshold {childMax.ReproductionThreshold} exceeds maximum {Settings.MaxPredatorReproductionThreshold}");
            }
        }

        [Fact]
        public void Mutate_ProducesVariation_NotIdenticalToParent()
        {
            // ARRANGE: Create parent with middle-range values (not at bounds)
            float parentSpeed = Settings.StartingPredatorSpeed;
            float parentSight = Settings.StartingPredatorSightRadius;
            float parentMetab = Settings.StartingPredatorMetabolismEfficiency;
            float parentRepro = Settings.StartingPredatorReproductionThreshold;

            // ACT: Create multiple children and check for variation
            bool foundSpeedVariation = false;
            bool foundSightVariation = false;
            bool foundMetabVariation = false;
            bool foundReproVariation = false;

            for (int i = 0; i < 30; i++)
            {
                PredatorGenome child = new PredatorGenome(
                    parentSpeed, parentSight, parentMetab, parentRepro
                );

                // Check if any trait differs from parent (allowing for small floating point errors)
                if (Math.Abs(child.Speed - parentSpeed) > 0.01f)
                    foundSpeedVariation = true;
                
                if (Math.Abs(child.SightRadius - parentSight) > 0.01f)
                    foundSightVariation = true;
                
                if (Math.Abs(child.MetabolismEfficiency - parentMetab) > 0.00001f)
                    foundMetabVariation = true;
                
                if (Math.Abs(child.ReproductionThreshold - parentRepro) > 0.01f)
                    foundReproVariation = true;
            }

            // ASSERT: At least some variation should occur across 30 children
            // (Mutation uses Settings.MutationRate, so statistically we should see variation)
            Assert.True(foundSpeedVariation, 
                "No variation found in Speed across 30 mutations");
            Assert.True(foundSightVariation, 
                "No variation found in SightRadius across 30 mutations");
            Assert.True(foundMetabVariation, 
                "No variation found in MetabolismEfficiency across 30 mutations");
            Assert.True(foundReproVariation, 
                "No variation found in ReproductionThreshold across 30 mutations");
        }

        [Fact]
        public void Constructor_WithParentValues_CreatesValidOffspring()
        {
            // ARRANGE: Parent values in mid-range
            float parentSpeed = 180f;
            float parentSight = 25f;
            float parentMetab = 0.006f;
            float parentRepro = 1800f;

            // ACT: Create offspring
            PredatorGenome offspring = new PredatorGenome(
                parentSpeed, parentSight, parentMetab, parentRepro
            );

            // ASSERT: Offspring traits should be close to parent (within mutation range)
            // Mutation is typically ±10%, so offspring should be within reasonable range
            float maxSpeedDiff = parentSpeed * Settings.PredatorMutationRate * 2; // Allow 2x mutation range
            float maxSightDiff = parentSight * Settings.PredatorMutationRate * 2;
            
            Assert.InRange(offspring.Speed, 
                parentSpeed - maxSpeedDiff, 
                parentSpeed + maxSpeedDiff);
            
            Assert.InRange(offspring.SightRadius,
                parentSight - maxSightDiff,
                parentSight + maxSightDiff);
        }

        [Fact]
        public void IGenome_Interface_IsImplementedCorrectly()
        {
            // ARRANGE & ACT
            PredatorGenome genome = new PredatorGenome();

            // ASSERT: Should implement IGenome interface
            Assert.IsAssignableFrom<IGenome>(genome);
            
            // Verify all IGenome properties are accessible
            float speed = genome.Speed;
            float sight = genome.SightRadius;
            float metab = genome.MetabolismEfficiency;
            float repro = genome.ReproductionThreshold;
            
            Assert.True(speed > 0, "Speed should be positive");
            Assert.True(sight > 0, "SightRadius should be positive");
            Assert.True(metab > 0, "MetabolismEfficiency should be positive");
            Assert.True(repro > 0, "ReproductionThreshold should be positive");
        }
    }
}