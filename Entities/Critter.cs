using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.delegates;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.Policies;
using Ecosystem_Simulator.Core.Structs;
namespace Ecosystem_Simulator.Entities
{
    public class Critter : AnimalEntityTemplate
    {
        
        public Critter(Vector2 startPos, CritterGenome dna, float Energy = Settings.CritterStartingEnergy) : base(startPos, dna, Energy)
        {
            _hungerThreshold = Settings.CritterHungerEnergyThreshold;
            _speedRatioWhenNotHungry = Settings.CritterSpeedRatioWhenNotHungry;
            _birthEnergyShareRatio = Settings.CritterBirthEnergyShareRatio;
            _eatDistance = Settings.EatDistance;
        }


        public void DebugInfo()
        {
            Console.WriteLine($"Critter {Id} - Pos: ({Position.X:F2}, {Position.Y:F2}), Energy: {Energy:F2}, Speed: {Speed:F2}, Sight: {SightRadius:F2}");
        }


        
        public override void SpawnChild()
        {
            float baby_energy = this.Energy * this._birthEnergyShareRatio;
            this.Energy -= baby_energy; //check if multiplying by 1-ratio is faster, need to make things more efficienct for heavier tests
            
            // Create new critter at parent position
            Critter baby = new Critter(this.Position, new CritterGenome(this.Speed,this.SightRadius,this.MetabolismEfficiency,this.ReproductionThreshold,true),baby_energy );

            // Raise spawn event with baby critter
            RequestSpawn(baby);
        }


    }
}