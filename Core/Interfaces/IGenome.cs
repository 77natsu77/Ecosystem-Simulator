namespace Ecosystem_Simulator.Core.Interfaces
{
    public interface IGenome
    {
        float GetGeneValue(string name);
        void Mutate();
    }
}
