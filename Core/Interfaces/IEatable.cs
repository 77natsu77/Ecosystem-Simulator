namespace Ecosystem_Simulator.Core.Interfaces
{
    public interface IEatable : IUpdatable
    {
        float EnergyValue { get; }
        void Consume(); // What happens when it gets eaten?
    }
}