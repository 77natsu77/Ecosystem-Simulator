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


            //Calculating change and adding it to parent genes
            float newSpeed = ParentSpeed + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentSpeed * Settings.MutationRate));
            float newSightRadius = ParentSightRadius + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentSightRadius * Settings.MutationRate));
            float newMetabolismEfficiency = ParentMetabolismEfficiency + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentMetabolismEfficiency * Settings.MutationRate));
            float newReproductionThreshold = ParentReproductionThreshold + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentReproductionThreshold * Settings.MutationRate));

            //clamping to max values
            this.Speed = Math.Max(Settings.MinCritterSpeed,Math.Min(Settings.MaxCritterSpeed, newSpeed));
            this.SightRadius = Math.Max(Settings.MinCritterSightradius,Math.Min(Settings.MaxCritterSightradius, newSightRadius));
            this.MetabolismEfficiency = Math.Max(Settings.MinCritterMetabolismEfficiency,Math.Min(Settings.MaxCritterMetabolismEfficiency, newMetabolismEfficiency));
            this.ReproductionThreshold = Math.Max(Settings.MinCritterReproductionThreshold,Math.Min(Settings.MaxCritterReproductionThreshold, newReproductionThreshold));
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
