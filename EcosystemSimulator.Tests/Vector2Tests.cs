using Ecosystem_Simulator.Core;
using Xunit;

namespace EcosystemSimulator.Tests
{
    /// <summary>
    /// Tests for the Vector2 struct to ensure correct construction and value storage.
    /// </summary>
    public class Vector2Tests
    {
        [Fact]
        public void Vector2_Construction_SetsXAndY()
        {
            // ARRANGE: Define expected values
            float expectedX = 42.5f;
            float expectedY = 73.2f;

            // ACT: Create a Vector2 with those values
            Vector2 vector = new Vector2(expectedX, expectedY);

            // ASSERT: Verify X and Y were stored correctly
            Assert.Equal(expectedX, vector.X);
            Assert.Equal(expectedY, vector.Y);
        }

        [Fact]
        public void Vector2_Construction_WithZeroValues_SetsCorrectly()
        {
            // ARRANGE & ACT
            Vector2 vector = new Vector2(0f, 0f);

            // ASSERT
            Assert.Equal(0f, vector.X);
            Assert.Equal(0f, vector.Y);
        }

        [Fact]
        public void Vector2_Construction_WithNegativeValues_SetsCorrectly()
        {
            // ARRANGE & ACT
            Vector2 vector = new Vector2(-10.5f, -25.3f);

            // ASSERT
            Assert.Equal(-10.5f, vector.X);
            Assert.Equal(-25.3f, vector.Y);
        }
    }
}