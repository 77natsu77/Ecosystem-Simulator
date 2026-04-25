using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EcosystemSimulator.Tests
{
    /// <summary>
    /// Tests for the SpatialHash class, which optimizes neighbor queries from O(n²) to O(1).
    /// </summary>
    public class SpatialHashTests
    {
        [Fact]
        public void GetHashKey_SamePosition_ReturnsSameKey()
        {
            // ARRANGE
            SpatialHash grid = new SpatialHash();
            Vector2 position1 = new Vector2(100f, 200f);
            Vector2 position2 = new Vector2(100f, 200f); // Identical position

            // ACT
            int key1 = grid.GetHashKey(position1);
            int key2 = grid.GetHashKey(position2);

            // ASSERT: Same position should always produce the same hash key
            Assert.Equal(key1, key2);
        }

        [Fact]
        public void GetHashKey_DifferentPositions_ReturnsDifferentKeys()
        {
            // ARRANGE
            SpatialHash grid = new SpatialHash();
            Vector2 position1 = new Vector2(100f, 200f);
            Vector2 position2 = new Vector2(500f, 600f); // Different cell

            // ACT
            int key1 = grid.GetHashKey(position1);
            int key2 = grid.GetHashKey(position2);

            // ASSERT: Distant positions should produce different keys
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void GetHashKey_PositionsInSameCell_ReturnsSameKey()
        {
            // ARRANGE: Cell size is 100, so positions within same 100x100 cell
            // should hash to the same key
            SpatialHash grid = new SpatialHash();
            Vector2 position1 = new Vector2(50f, 50f);   // In cell (0,0)
            Vector2 position2 = new Vector2(99f, 99f);   // Also in cell (0,0)

            // ACT
            int key1 = grid.GetHashKey(position1);
            int key2 = grid.GetHashKey(position2);

            // ASSERT
            Assert.Equal(key1, key2);
        }

        [Fact]
        public void GetEntitiesInRadius_ReturnsOnlyNearby()
        {
            // ARRANGE: Create a spatial hash and add entities at known positions
            SpatialHash grid = new SpatialHash();

            // Create critters at specific positions
            Vector2 centerPos = new Vector2(500f, 500f);
            Vector2 nearbyPos = new Vector2(520f, 520f);  // About 28 units away
            Vector2 farPos = new Vector2(900f, 900f);     // About 565 units away

            DefaultGenome genome = new DefaultGenome();
            Critter centerCritter = new Critter(centerPos, genome);
            Critter nearbyCritter = new Critter(nearbyPos, genome);
            Critter farCritter = new Critter(farPos, genome);

            // Register all critters
            grid.Register(centerCritter);
            grid.Register(nearbyCritter);
            grid.Register(farCritter);

            // ACT: Query for entities within 100 units of center
            HashSet<IEntity> results = grid.GetEntitiesInRadius(centerPos, 100f);

            // ASSERT: Should find center and nearby, but NOT far
            Assert.Contains(centerCritter, results);
            Assert.Contains(nearbyCritter, results);
            Assert.DoesNotContain(farCritter, results);
        }

        [Fact]
        public void GetEntitiesInRadius_EmptyGrid_ReturnsEmptySet()
        {
            // ARRANGE
            SpatialHash grid = new SpatialHash();
            Vector2 queryPos = new Vector2(100f, 100f);

            // ACT: Query an empty grid
            HashSet<IEntity> results = grid.GetEntitiesInRadius(queryPos, 50f);

            // ASSERT: Should return empty set
            Assert.Empty(results);
        }

        [Fact]
        public void RegisterAndUnregister_WorksCorrectly()
        {
            // ARRANGE
            SpatialHash grid = new SpatialHash();
            Vector2 position = new Vector2(100f, 100f);
            DefaultGenome genome = new DefaultGenome();
            Critter critter = new Critter(position, genome);

            // ACT: Register the critter
            grid.Register(critter);
            HashSet<IEntity> resultsAfterRegister = grid.GetEntitiesInRadius(position, 50f);

            // Unregister the critter
            grid.Unregister(critter);
            HashSet<IEntity> resultsAfterUnregister = grid.GetEntitiesInRadius(position, 50f);

            // ASSERT
            Assert.Contains(critter, resultsAfterRegister);
            Assert.DoesNotContain(critter, resultsAfterUnregister);
        }

        [Fact]
        public void GetEntitiesInRadius_MultipleEntitiesInSameCell_ReturnsAll()
        {
            // ARRANGE: Put multiple entities in the same cell
            SpatialHash grid = new SpatialHash();
            Vector2 pos1 = new Vector2(50f, 50f);
            Vector2 pos2 = new Vector2(60f, 60f);
            Vector2 pos3 = new Vector2(70f, 70f);

            DefaultGenome genome = new DefaultGenome();
            Critter critter1 = new Critter(pos1, genome);
            Critter critter2 = new Critter(pos2, genome);
            Critter critter3 = new Critter(pos3, genome);

            grid.Register(critter1);
            grid.Register(critter2);
            grid.Register(critter3);

            // ACT
            HashSet<IEntity> results = grid.GetEntitiesInRadius(pos1, 100f);

            // ASSERT: Should find all three
            Assert.Equal(3, results.Count);
            Assert.Contains(critter1, results);
            Assert.Contains(critter2, results);
            Assert.Contains(critter3, results);
        }
    }
}