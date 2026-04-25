using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Entities;
using System.Collections.Generic;
using Xunit;

namespace EcosystemSimulator.Tests
{
    /// <summary>
    /// Tests for the FoodPellet entity.
    /// </summary>
    public class FoodPelletTests
    {
        [Fact]
        public void Constructor_SetsPositionCorrectly()
        {
            // ARRANGE
            Vector2 expectedPosition = new Vector2(123.45f, 678.90f);

            // ACT
            FoodPellet food = new FoodPellet(expectedPosition);

            // ASSERT
            Assert.Equal(expectedPosition.X, food.Position.X);
            Assert.Equal(expectedPosition.Y, food.Position.Y);
        }

        [Fact]
        public void Consume_MarksForRemoval()
        {
            // ARRANGE
            Vector2 position = new Vector2(100f, 100f);
            FoodPellet food = new FoodPellet(position);

            Assert.False(food.IsPendingRemoval, "Food should not be pending removal initially");

            // ACT
            food.Consume();

            // ASSERT
            Assert.True(food.IsPendingRemoval, "Food should be marked for removal after being consumed");
        }

        [Fact]
        public void EnergyValue_MatchesSettings()
        {
            // ARRANGE & ACT
            Vector2 position = new Vector2(100f, 100f);
            FoodPellet food = new FoodPellet(position);

            // ASSERT: Energy value should match the configured value
            Assert.Equal(Settings.FoodPelletEnergyValue, food.EnergyValue);
        }

        [Fact]
        public void Update_CanBeCalledWithoutError()
        {
            // ARRANGE
            Vector2 position = new Vector2(100f, 100f);
            FoodPellet food = new FoodPellet(position);

            // ACT & ASSERT: Update should not throw
            var exception = Record.Exception(() =>
            {
                food.Update(deltaTime: 0.016, nearby: new List<Ecosystem_Simulator.Core.Interfaces.IEntity>());
            });

            Assert.Null(exception);
        }
    }
}