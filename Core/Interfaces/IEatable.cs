namespace Ecosystem_Simulator.Core.Interfaces
{
    public interface IEatable
    {
        float NutritionalValue { get; }
        void Consume(); // What happens when it gets eaten?
    }
}