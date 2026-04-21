namespace Ecosystem_Simulator.Core.Interfaces
{
    public interface IEnergyPolicy
    {
        float CalculateLoss(Vector2 velocity, float SightRadius, double deltaTime);
    }
}
