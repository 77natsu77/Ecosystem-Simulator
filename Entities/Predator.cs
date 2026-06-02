// An entity which hunts other animal entities...
using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Structs;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Entities;

public class Predator: AnimalEntityTemplate
{
    
    // Might change this as it checks energy every frame, which could be costly, but it is needed to determine if the predator should be in cannibal mode or not, which drastically changes its behavior and is a key part of the simulation, so I think it is worth it, especially since it is just a simple float comparison
    // Adding a giick where sight radius increases by 1.,5 when cannibalistuic to make them more likely to find other predators to eat and get out of cannibal mode faster
    public bool CannibalMode => functionCannibalMode();
    public bool SightRadiusBuffed { get; private set; } = false; // Track if the sight radius has been buffed for cannibal mode
    private bool functionCannibalMode(){ // Check if energy is below the cannibal threshold and adjust sight radius accordingly
        if (Energy <= Settings.PredatorCannibalThreshold)
        {
            if (!SightRadiusBuffed)
            {
                SightRadius = SightRadius * Settings.PredatorCannibalSightRadiusBuff; // Increase sight radius  when in cannibal mode to help find other predators to eat
                SightRadiusBuffed = true;
            }
            return true;
        }
        else
        {
            SightRadius = _dna.SightRadius; // Reset sight radius to original value from DNA when not in cannibal mode
            SightRadiusBuffed = false;
            return false;
        }
    }

    public Predator(Vector2 startPos, PredatorGenome dna, float Energy = Settings.PredatorStartingEnergy) : base(startPos, dna, Energy)
    {
        _hungerThreshold = Settings.PredatorHungerEnergyThreshold;
        _speedRatioWhenNotHungry = Settings.PredatorSpeedRatioWhenNotHungry;
        _birthEnergyShareRatio = Settings.PredatorBirthEnergyShareRatio;
        _eatDistance = Settings.EatDistance;
    }

    public override void SpawnChild()
    {
        float baby_energy = this.Energy * Settings.PredatorBirthEnergyShareRatio;
        this.Energy -= baby_energy; 
        
        // Create new baby at parent position
        Predator baby = new Predator(this.Position, new PredatorGenome(this.Speed,this.SightRadius,this.MetabolismEfficiency,this.ReproductionThreshold,true),baby_energy );

        //  Trigger spawn event
        RequestSpawn(baby);
    }
   

    public void DebugInfo()
        {
            Console.WriteLine($"Predator {Id} - Pos: ({Position.X:F2}, {Position.Y:F2}), Energy: {Energy:F2}, Speed: {Speed:F2}, Sight: {SightRadius:F2}");
        }

    // Overiding this to implement canibal logic specific to predators
    public override void SenseSurroundings(IEnumerable<IEntity> nearbyEntities, ref IEatable closestFood)
    {
        float minDistanceSq = float.MaxValue;
        float eatDistSq = _eatDistance * _eatDistance;
        float sightRadiusSq = SightRadius * SightRadius;
        //loop through nearby entities and eat if close enough, sense closest food otherwise
        foreach (IEntity entity in nearbyEntities) // Detect stimuli and provide suitable response
        {
            if (entity != this && !entity.IsPendingRemoval)
            {
                float dX = entity.Position.X - this.Position.X;
                float dY = entity.Position.Y - this.Position.Y;
                float distanceSq = dX * dX + dY * dY;

                // ACTION 1: EATING
                if (distanceSq < eatDistSq)
                {
                    if (entity is Predator p && this.CannibalMode)
                    {
                        p.Death();
                        this.Energy += p.Energy * Settings.PredatorEnergyGainFromConsumption; // For simplicity, using the same energy gain for cannibalism as for eating critters, but this could be adjusted to be different if needed
                    }
                    else if (entity is AnimalEntityTemplate a) // Can eat any other animal entity, including other predators if in cannibal mode
                   {
                        a.Death();
                        this.Energy += a.Energy * Settings.PredatorEnergyGainFromConsumption;
                   }
                     
                }

                // ACTION 2: SENSING FOOD (Only if hungry)
                else if (distanceSq < sightRadiusSq && distanceSq < minDistanceSq)
                {
                    // TODO: PLEASE make a check which is more efficient than this
                    bool isTarget = 
                    ((entity is AnimalEntityTemplate) && (entity is not Predator)) ||
                    (entity is Predator p && p != this && this.CannibalMode);
                    if (isTarget)
                    {
                        minDistanceSq = distanceSq;
                        closestFood = entity;
                    }
                }
            }
        }
    }
}