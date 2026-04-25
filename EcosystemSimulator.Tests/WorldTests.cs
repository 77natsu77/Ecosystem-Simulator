using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using System.Linq;
using Xunit;

namespace EcosystemSimulator.Tests
{
    /// <summary>
    /// Tests for the World class, which manages the entire simulation state.
    /// </summary>
    public class WorldTests
    {
        [Fact]
        public void Spawn_AddsEntityToWorld()
        {
            // ARRANGE
            World world = new World(800f, 600f);
            Vector2 position = new Vector2(100f, 100f);
            DefaultGenome genome = new DefaultGenome();
            Critter critter = new Critter(position, genome);

            int entitiesBeforeSpawn = world.Entities.Count();

            // ACT
            world.Spawn(critter);

            // ASSERT
            Assert.Equal(entitiesBeforeSpawn + 1, world.Entities.Count());
            Assert.Contains(critter, world.Entities);
        }

        [Fact]
        public void Seed_CreatesCorrectNumberOfEntities()
        {
            // ARRANGE
            World world = new World(1200f, 800f);
            int expectedCritters = 10;
            int expectedFood = 20;
            int expectedTotal = expectedCritters + expectedFood;

            // ACT
            world.Seed(expectedCritters, expectedFood);

            // ASSERT
            Assert.Equal(expectedTotal, world.Entities.Count());
            
            // Verify correct types were spawned
            int actualCritters = world.Entities.OfType<Critter>().Count();
            int actualFood = world.Entities.OfType<FoodPellet>().Count();
            
            Assert.Equal(expectedCritters, actualCritters);
            Assert.Equal(expectedFood, actualFood);
        }

        [Fact]
        public void Tick_RemovesPendingEntities()
        {
            // ARRANGE: Create a critter with almost no energy (will die)
            World world = new World(800f, 600f);
            Vector2 position = new Vector2(100f, 100f);
            DefaultGenome genome = new DefaultGenome();
            
            // Very low energy - will die after tick
            Critter dyingCritter = new Critter(position, genome, Energy: 0.1f);
            world.Spawn(dyingCritter);

            int entitiesBeforeTick = world.Entities.Count();

            // ACT: Run simulation tick (critter will run out of energy)
            world.Tick(deltaTime: 1.0);

            // ASSERT: Dead critter should be removed
            Assert.True(dyingCritter.IsPendingRemoval,
                "Critter should be marked for removal");
            
            Assert.True(world.Entities.Count() < entitiesBeforeTick,
                "Dead entities should be removed from world");
            
            Assert.DoesNotContain(dyingCritter, world.Entities);
        }

        [Fact]
        public void Tick_UpdatesAllEntities()
        {
            // ARRANGE: Create world with critters and food
            World world = new World(800f, 600f);
            world.Seed(critterCount: 5, foodCount: 10);

            // Get initial positions
            var initialPositions = world.Entities.OfType<Critter>()
                .Select(c => new { Critter = c, Pos = c.Position })
                .ToList();

            // ACT: Run multiple ticks
            for (int i = 0; i < 10; i++)
            {
                world.Tick(deltaTime: 0.016);
            }

            // ASSERT: At least some critters should have moved
            bool someMovement = initialPositions.Any(initial =>
            {
                var currentPos = initial.Critter.Position;
                return currentPos.X != initial.Pos.X || currentPos.Y != initial.Pos.Y;
            });

            Assert.True(someMovement,
                "Critters should move during simulation ticks");
        }

        [Fact]
        public void Constructor_SetsDimensionsCorrectly()
        {
            // ARRANGE & ACT
            float expectedWidth = 1920f;
            float expectedHeight = 1080f;
            World world = new World(expectedWidth, expectedHeight);

            // ASSERT
            Assert.Equal(expectedWidth, world.Width);
            Assert.Equal(expectedHeight, world.Height);
        }

        [Fact]
        public void Tick_HandlesEmptyWorld_WithoutError()
        {
            // ARRANGE: Empty world
            World world = new World(800f, 600f);

            // ACT & ASSERT: Should not throw
            var exception = Record.Exception(() =>
            {
                world.Tick(deltaTime: 0.016);
            });

            Assert.Null(exception);
        }

        [Fact]
        public void RequestSpawn_AddsToSpawnQueue_ProcessedOnNextTick()
        {
            // ARRANGE: World with one critter that will reproduce
            World world = new World(800f, 600f);
            Vector2 position = new Vector2(100f, 100f);
            DefaultGenome genome = new DefaultGenome();
            
            // Give critter enough energy to reproduce
            float reproEnergy = Settings.StartingCritterReproductionThreshold + 500f;
            Critter parent = new Critter(position, genome, Energy: reproEnergy);
            world.Spawn(parent);

            int entitiesBeforeTick = world.Entities.Count();

            // ACT: Tick - parent should reproduce
            world.Tick(deltaTime: 0.016);

            // ASSERT: Should have spawned offspring
            Assert.True(world.Entities.Count() > entitiesBeforeTick,
                "World should contain offspring after reproduction");
        }
    }
}