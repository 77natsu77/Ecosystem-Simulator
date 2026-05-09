namespace Ecosystem_Simulator.Core.Interfaces
{
    public interface IGenome
    {
        float GetGeneValue(string name);
        void Mutate();
         public float Speed { get;  }
        public float SightRadius { get;}
        public float MetabolismEfficiency { get;  }
        public float ReproductionThreshold { get;}
    }
}
