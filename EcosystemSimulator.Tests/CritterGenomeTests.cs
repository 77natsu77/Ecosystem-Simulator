using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using System;
using Xunit;

namespace EcosystemSimulator.Tests
{
    /// <summary>
    /// Tests for the CritterGenome class, focusing on mutation logic and boundary conditions.
    /// Validates that genetic mutations stay within viable ranges for critter evolution.
    /// </summary>
    public class CritterGenomeTests
    {
        [Fact]
        public void DefaultConstructor_SetsStartingValues()
        {
            // ACT: Create genome with default constructor
            CritterGenome genome = new CritterGenome();

            // ASSERT: Should have exact starting values (no mutation)
            Assert.Equal(Settings.StartingCritterSpeed, genome.Speed);
            Assert.Equal(Settings.StartingCritterSightRadius, genome.SightRadius);
            Assert.Equal(Settings.StartingCritterMetabolismEfficiency, genome.MetabolismEfficiency);
            Assert.Equal(Settings.StartingCritterReproductionThreshold, genome.ReproductionThreshold);
        }

        [Fact]
        public void Mutate_ClampsToBounds_Speed()
        {
            // ARRANGE: Parent with speed at maximum (should mutate but stay clamped)
            float parentSpeed = Settings.MaxCritterSpeed;
            float parentSight = Settings.StartingCritterSightRadius;
            float parentMetab = Settings.StartingCritterMetabolismEfficiency;
            float parentRepro = Settings.StartingCritterReproductionThreshold;

            // ACT: Create multiple children to test upper bound clamping
            for (int i = 0; i < 20; i++)
            {
                CritterGenome child = new CritterGenome(
                    parentSpeed, parentSight, parentMetab, parentRepro
                );

                // ASSERT: Child's speed should never exceed max
                Assert.True(child.Speed <= Settings.MaxCritterSpeed,
                    $"Speed {child.Speed} exceeds maximum {Settings.MaxCritterSpeed}");
                
                // Also verify it doesn't go below minimum
                Assert.True(child.Speed >= Settings.MinCritterSpeed,
                    $"Speed {child.Speed} is below minimum {Settings.MinCritterSpeed}");
            }
        }

        [Fact]
        public void Mutate_ClampsToBounds_SightRadius()
        {
            // ARRANGE: Test both extremes
            float parentSpeed = Settings.StartingCritterSpeed;
            float parentSightMin = Settings.MinCritterSightradius;  // At minimum
            float parentSightMax = Settings.MaxCritterSightradius;  // At maximum
            float parentMetab = Settings.StartingCritterMetabolismEfficiency;
            float parentRepro = Settings.StartingCritterReproductionThreshold;

            // ACT & ASSERT: Test minimum bound
            for (int i = 0; i < 20; i++)
            {
                CritterGenome childMin = new CritterGenome(
                    parentSpeed, parentSightMin, parentMetab, parentRepro
                );
                
                Assert.True(childMin.SightRadius >= Settings.MinCritterSightradius,
                    $"SightRadius {childMin.SightRadius} is below minimum {Settings.MinCritterSightradius}");
            }

            // ACT & ASSERT: Test maximum bound
            for (int i = 0; i < 20; i++)
            {
                CritterGenome childMax = new CritterGenome(
                    parentSpeed, parentSightMax, parentMetab, parentRepro
                );
                
                Assert.True(childMax.SightRadius <= Settings.MaxCritterSightradius,
                    $"SightRadius {childMax.SightRadius} exceeds maximum {Settings.MaxCritterSightradius}");
            }
        }

        [Fact]
        public void Mutate_ClampsToBounds_MetabolismEfficiency()
        {
            // ARRANGE
            float parentSpeed = Settings.StartingCritterSpeed;
            float parentSight = Settings.StartingCritterSightRadius;
            float parentMetabMin = Settings.MinCritterMetabolismEfficiency;  // At minimum
            float parentMetabMax = Settings.MaxCritterMetabolismEfficiency;  // At maximum
            float parentRepro = Settings.StartingCritterReproductionThreshold;

            // ACT & ASSERT: Test minimum bound
            for (int i = 0; i < 20; i++)
            {
                CritterGenome childMin = new CritterGenome(
                    parentSpeed, parentSight, parentMetabMin, parentRepro
                );
                
                Assert.True(childMin.MetabolismEfficiency >= Settings.MinCritterMetabolismEfficiency,
                    $"MetabolismEfficiency {childMin.MetabolismEfficiency} is below minimum {Settings.MinCritterMetabolismEfficiency}");
            }

            // ACT & ASSERT: Test maximum bound
            for (int i = 0; i < 20; i++)
            {
                CritterGenome childMax = new CritterGenome(
                    parentSpeed, parentSight, parentMetabMax, parentRepro
                );
                
                Assert.True(childMax.MetabolismEfficiency <= Settings.MaxCritterMetabolismEfficiency,
                    $"MetabolismEfficiency {childMax.MetabolismEfficiency} exceeds maximum {Settings.MaxCritterMetabolismEfficiency}");
            }
        }

        [Fact]
        public void Mutate_ClampsToBounds_ReproductionThreshold()
        {
            // ARRANGE
            float parentSpeed = Settings.StartingCritterSpeed;
            float parentSight = Settings.StartingCritterSightRadius;
            float parentMetab = Settings.StartingCritterMetabolismEfficiency;
            float parentReproMin = Settings.MinCritterReproductionThreshold;  // At minimum
            float parentReproMax = Settings.MaxCritterReproductionThreshold;  // At maximum

            // ACT & ASSERT: Test minimum bound
            for (int i = 0; i < 20; i++)
            {
                CritterGenome childMin = new CritterGenome(
                    parentSpeed, parentSight, parentMetab, parentReproMin
                );
                
                Assert.True(childMin.ReproductionThreshold >= Settings.MinCritterReproductionThreshold,
                    $"ReproductionThreshold {childMin.ReproductionThreshold} is below minimum {Settings.MinCritterReproductionThreshold}");
            }

            // ACT & ASSERT: Test maximum bound
            for (int i = 0; i < 20; i++)
            {
                CritterGenome childMax = new CritterGenome(
                    parentSpeed, parentSight, parentMetab, parentReproMax
                );
                
                Assert.True(childMax.ReproductionThreshold <= Settings.MaxCritterReproductionThreshold,
                    $"ReproductionThreshold {childMax.ReproductionThreshold} exceeds maximum {Settings.MaxCritterReproductionThreshold}");
            }
        }

        [Fact]
        public void Mutate_ProducesVariation_NotIdenticalToParent()
        {
            // ARRANGE: Create parent with middle-range values (not at bounds)
            float parentSpeed = 150f;
            float parentSight = 60f;
            float parentMetab = 0.003f;
            float parentRepro = 2000f;

            // ACT: Create multiple children and check for variation
            bool foundSpeedVariation = false;
            bool foundSightVariation = false;
            bool foundMetabVariation = false;
            bool foundReproVariation = false;

            for (int i = 0; i < 30; i++)
            {
                CritterGenome child = new CritterGenome(
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
            float parentSpeed = 120f;
            float parentSight = 45f;
            float parentMetab = 0.0025f;
            float parentRepro = 1500f;

            // ACT: Create offspring
            CritterGenome offspring = new CritterGenome(
                parentSpeed, parentSight, parentMetab, parentRepro
            );

            // ASSERT: Offspring traits should be close to parent (within mutation range)
            // Mutation is typically ±10%, so offspring should be within reasonable range
            float maxSpeedDiff = parentSpeed * Settings.CritterMutationRate * 2; // Allow 2x mutation range
            float maxSightDiff = parentSight * Settings.CritterMutationRate * 2;
            
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
            CritterGenome genome = new CritterGenome();

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

        [Fact]
        public void CritterGenome_DifferentFromPredatorGenome_StartingValues()
        {
            // ARRANGE & ACT
            CritterGenome critterGenome = new CritterGenome();
            PredatorGenome predatorGenome = new PredatorGenome();

            // ASSERT: Starting values should be different for different species
            // (This validates that critters and predators have distinct evolutionary paths)
            bool hasDifferentTraits = 
                critterGenome.Speed != predatorGenome.Speed ||
                critterGenome.SightRadius != predatorGenome.SightRadius ||
                critterGenome.MetabolismEfficiency != predatorGenome.MetabolismEfficiency ||
                critterGenome.ReproductionThreshold != predatorGenome.ReproductionThreshold;

            Assert.True(hasDifferentTraits,
                "Critter and Predator genomes should have different starting values");
        }
    }
}