using Ecosystem_Simulator.Core.Interfaces;
using System;
namespace Ecosystem_Simulator.Core
{
    public class SmartyGenome : IGenome
    {
        public float Speed { get; private set; }
        public float SightRadius { get; private set; }
        public float MetabolismEfficiency { get; private set; }
        public float ReproductionThreshold { get; private set; }
        public float PathUpdateInterval { get; private set; }
        public float GetGeneValue(string name)
        {
            return 0;
        }
        public void Mutate() { }
        public void Mutate(float ParentSpeed, float ParentSightRadius, float ParentMetabolismEfficiency, float ParentReproductionThreshold, float ParentPathUpdateInterval)
        {

            
            //Calculating change and adding it to parent genes
            float newSpeed = ParentSpeed + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentSpeed * Settings.SmartyMutationRate));
            float newSightRadius = ParentSightRadius + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentSightRadius * Settings.SmartyMutationRate));
            float newMetabolismEfficiency = ParentMetabolismEfficiency + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentMetabolismEfficiency * Settings.SmartyMutationRate));
            float newReproductionThreshold = ParentReproductionThreshold + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentReproductionThreshold * Settings.SmartyMutationRate));
            float newPathUpdateInterval = ParentPathUpdateInterval + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentPathUpdateInterval * Settings.SmartyMutationRate));

            //clamping to max values
            this.Speed = Math.Max(Settings.MinSmartySpeed,Math.Min(Settings.MaxSmartySpeed, newSpeed));
            this.SightRadius = Math.Max(Settings.MinSmartySightRadius,Math.Min(Settings.MaxSmartySightRadius, newSightRadius));
            this.MetabolismEfficiency = Math.Max(Settings.MinSmartyMetabolismEfficiency,Math.Min(Settings.MaxSmartyMetabolismEfficiency, newMetabolismEfficiency));
            this.ReproductionThreshold = Math.Max(Settings.MinSmartyReproductionThreshold,Math.Min(Settings.MaxSmartyReproductionThreshold, newReproductionThreshold));
            this.PathUpdateInterval = Math.Max(Settings.SmartyMinimumPathUpdateInterval, Math.Min(Settings.StartingSmartyPathUpdateInterval, newPathUpdateInterval));
        }

        public SmartyGenome(float Speed, float SightRadius, float MetabolismEfficiency, float ReproductionThreshold, float PathUpdateInterval, bool newBorn = false)
        {
            if (newBorn)
            {
                Mutate(Speed,SightRadius,MetabolismEfficiency,ReproductionThreshold,PathUpdateInterval);
            }
            else
            {
                this.Speed = Speed;
                this.SightRadius = SightRadius;
                this.MetabolismEfficiency = MetabolismEfficiency;
                this.ReproductionThreshold = ReproductionThreshold;
                this.PathUpdateInterval = PathUpdateInterval;
            }
            
        }

        public SmartyGenome()
        {
            Speed = Settings.StartingSmartySpeed;
            SightRadius = Settings.StartingSmartySightRadius;
            MetabolismEfficiency = Settings.StartingSmartyMetabolismEfficiency;
            ReproductionThreshold = Settings.StartingSmartyReproductionThreshold;
            PathUpdateInterval = Settings.StartingSmartyPathUpdateInterval;
        }

    }
}
