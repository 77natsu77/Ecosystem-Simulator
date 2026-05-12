namespace Ecosystem_Simulator.Core.Interfaces
{
    public interface IEatable : IUpdatable
    {
        float EnergyValue { get; }
        float Consume() => EnergyValue; // return energy value and allow for any additional logic in the future, such as partial consumption or regeneration, without changing the interface
    }
}