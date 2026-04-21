using Ecosystem_Simulator.Core.Interfaces;
using System;
namespace Ecosystem_Simulator.Core
{
    public class DefaultGenome : IGenome
    {
        public float Speed { get; private set; }
        public float SightRadius { get; private set; }
        public float MetabolismEfficiency { get; private set; }
        public float ReproductionThreshold { get; private set; }
        public float GetGeneValue(string name)
        {
            return 0;
        }
        public void Mutate() { }
        public void Mutate(float ParentSpeed, float ParentSightRadius, float ParentMetabolismEfficiency, float ParentReproductionThreshold)
        {
            //Mutation Logic: Each attribute can vary by +/ -10 %

            //Calculating change and adding it to parent genes
            float newSpeed = ParentSpeed + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentSpeed * 0.1));
            float newSightRadius = ParentSightRadius + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentSightRadius * 0.1));
            float newMetabolismEfficiency = ParentMetabolismEfficiency + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentMetabolismEfficiency * 0.1));
            float newReproductionThreshold = ParentReproductionThreshold + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentReproductionThreshold * 0.1));

            //clamping to max values
            this.Speed = Math.Max(20f,Math.Min(Settings.MaxCritterSpeed, newSpeed));
            this.SightRadius = Math.Max(10f,Math.Min(Settings.MaxCritterSightradius, newSightRadius));
            this.MetabolismEfficiency = Math.Max(0.0001f,Math.Min(Settings.MaxCritterMetabolismEfficiency, newMetabolismEfficiency));
            this.ReproductionThreshold = Math.Max(0,Math.Min(Settings.MaxCritterReproductionThreshold, newReproductionThreshold));
        }

        public DefaultGenome(float ParentSpeed, float ParentSightRadius, float ParentMetabolismEfficiency, float ParentReproductionThreshold)
        {
            Mutate(ParentSpeed,  ParentSightRadius,  ParentMetabolismEfficiency,  ParentReproductionThreshold);
        }

        public DefaultGenome()
        {
            Speed = Settings.StartingCritterSpeed;
            SightRadius = Settings.StartingCritterSightRadius;
            MetabolismEfficiency = Settings.StartingCritterMetabolismEfficiency;
            ReproductionThreshold = Settings.StartingCritterReproductionThreshold;
        }

    }
}
