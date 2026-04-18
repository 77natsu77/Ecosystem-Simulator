using Ecosystem_Simulator.Core.Interfaces;
using System;
namespace Ecosystem_Simulator.Core
{
    public class DefaultGenome : IGenome
    {
        public float GetGeneValue(string name)
        {
            return 0;
        }
        public void Mutate()
        {
            Console.WriteLine("Mutated");
        }
    }
}
