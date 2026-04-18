using Ecosystem_Simulator.Core.Interfaces;

namespace Ecosystem_Simulator.Core.Policies
{
    public class StandardMetabolism : IEnergyPolicy
    {
        public float CalculateLoss(Vector2 velocity, double deltaTime)
        {
            // Base cost + (v^2 * multiplier)
            // We multiply by deltaTime so the loss is "per second"
            float speedSquared = (velocity.X * velocity.X) + (velocity.Y * velocity.Y);
            float totalLoss = (Settings.BaseMetabolism + (speedSquared * Settings.MovementMultiplier)) * (float)deltaTime;

            return totalLoss;
        }
    }
}