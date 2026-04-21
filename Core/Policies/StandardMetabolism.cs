using Ecosystem_Simulator.Core.Interfaces;

namespace Ecosystem_Simulator.Core.Policies
{
    public class StandardMetabolism : IEnergyPolicy
    {
        private float _metabolismEfficiency;
        public float CalculateLoss(Vector2 velocity, float SightRadius, double deltaTime)
        {
            // We multiply by deltaTime so the loss is "per second"

            float speedSquared = (velocity.X * velocity.X) + (velocity.Y * velocity.Y);
            float MovementCost = speedSquared * _metabolismEfficiency;
            float SightCost = SightRadius * _metabolismEfficiency;
            float totalLoss = (Settings.BaseMetabolism + MovementCost + SightCost) * (float)deltaTime;

            return totalLoss;
        }

        public StandardMetabolism(float efficicency) => _metabolismEfficiency = efficicency;
    }
}