using Ecosystem_Simulator.Core.Interfaces;
using System;
namespace Ecosystem_Simulator.Core
{
    public class PredatorGenome : IGenome
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
            float newSpeed = ParentSpeed + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentSpeed * Settings.PredatorMutationRate));
            float newSightRadius = ParentSightRadius + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentSightRadius * Settings.PredatorMutationRate));
            float newMetabolismEfficiency = ParentMetabolismEfficiency + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentMetabolismEfficiency * Settings.PredatorMutationRate));
            float newReproductionThreshold = ParentReproductionThreshold + (float)((Settings.Rng.NextDouble() * 2 - 1) * (ParentReproductionThreshold * Settings.PredatorMutationRate));

            //clamping to max values
            this.Speed = Math.Max(Settings.MinPredatorSpeed,Math.Min(Settings.MaxPredatorSpeed, newSpeed));
            this.SightRadius = Math.Max(Settings.MinPredatorSightradius,Math.Min(Settings.MaxPredatorSightradius, newSightRadius));
            this.MetabolismEfficiency = Math.Max(Settings.MinPredatorMetabolismEfficiency,Math.Min(Settings.MaxPredatorMetabolismEfficiency, newMetabolismEfficiency));
            this.ReproductionThreshold = Math.Max(Settings.MinPredatorReproductionThreshold,Math.Min(Settings.MaxPredatorReproductionThreshold, newReproductionThreshold));
        }

        public PredatorGenome(float Speed, float SightRadius, float MetabolismEfficiency, float ReproductionThreshold, bool newBorn = false)
        {
            if (newBorn)
            {
                Mutate(Speed,SightRadius,MetabolismEfficiency,ReproductionThreshold);
            }
            else
            {
                this.Speed = Speed;
                this.SightRadius = SightRadius;
                this.MetabolismEfficiency = MetabolismEfficiency;
                this.ReproductionThreshold = ReproductionThreshold;
            }
            
        }

        public PredatorGenome()
        {
            Speed = Settings.StartingPredatorSpeed;
            SightRadius = Settings.StartingPredatorSightRadius;
            MetabolismEfficiency = Settings.StartingPredatorMetabolismEfficiency;
            ReproductionThreshold = Settings.StartingPredatorReproductionThreshold;
        }

    }
}
