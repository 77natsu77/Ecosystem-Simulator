//an entity which hunts critters
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Structs;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.Policies;
using Ecosystem_Simulator.Core.delegates;
using Ecosystem_Simulator.Entities;

public class Predator: IUpdatable, IMovable
{
    private readonly IEnergyPolicy _metabolism;
    private readonly IGenome _dna;

    private float _wanderAngle;
    public float SightRadius { get; private set; }
    public float ReproductionThreshold { get; private set; }
    public float MetabolismEfficiency { get; private set; }

    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; private set; }
    public float Speed { get; private set; }
    public float Energy { get; private set; }
    public bool IsPendingRemoval { get; private set; }
    public int Id { get; private set; } // Unique identifier for the critter, set in constructor
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

    public event SpawnRequestDelegate OnSpawnRequested;

    public Predator(Vector2 startPos, PredatorGenome dna, float Energy = Settings.PredatorStartingEnergy)
    {
        this.Position = startPos;
        this._dna = dna;
        this.Energy = Energy;
        this.Id = this.GetHashCode(); // Assign a unique ID to the predator

        this.Speed = dna.Speed;
        this.SightRadius = dna.SightRadius;
        this.MetabolismEfficiency = dna.MetabolismEfficiency;
        this.ReproductionThreshold = dna.ReproductionThreshold;

        this._metabolism = new StandardMetabolism(MetabolismEfficiency); 

        // Set initial velocity using the new unique speed
        float angle = (float)(Settings.Rng.NextDouble() * Math.PI * 2);
        this.Velocity = new Vector2((float)Math.Cos(angle) * this.Speed, (float)Math.Sin(angle) * this.Speed);
    }

    public void Update(double deltaTime, IEnumerable<IEntity> nearbyEntities)
        {
            DetectCollisions(nearbyEntities);
            if (!this.IsPendingRemoval)
            {
                ProcessStimuli(deltaTime, nearbyEntities);
            }
        }
    public void SpawnChild()
    {
        float baby_energy = this.Energy * Settings.PredatorBirthEnergyShareRatio;
        this.Energy -= baby_energy; 
        
        // Create new baby at parent position
        Predator baby = new Predator(this.Position, new PredatorGenome(this.Speed,this.SightRadius,this.MetabolismEfficiency,this.ReproductionThreshold,true),baby_energy );

        //  Trigger spawn event
        OnSpawnRequested?.Invoke(baby);
    }
    public void InvertVelocityX()
        {
            Vector2 newVelocity = new Vector2();
            newVelocity.X = -this.Velocity.X;
            newVelocity.Y = this.Velocity.Y;
            this.Velocity = newVelocity;
            // Sync the wander angle to the new direction
            _wanderAngle = (float)Math.Atan2(this.Velocity.Y, this.Velocity.X);
        }
    public void InvertVelocityY()
        {
            Vector2 newVelocity = new Vector2();
            newVelocity.X = this.Velocity.X;
            newVelocity.Y = -this.Velocity.Y;
            this.Velocity = newVelocity;
            // Sync the wander angle to the new direction
            _wanderAngle = (float)Math.Atan2(this.Velocity.Y, this.Velocity.X);
        }
    public void ApplyMovement(double deltaTime)
        {
            Vector2 newPos = new Vector2();
            newPos.X = (float)(this.Position.X + (this.Velocity.X * deltaTime));
            newPos.Y = (float)(this.Position.Y + (this.Velocity.Y * deltaTime));
            this.Position = newPos;
        }

    private void SteerTowards(Vector2 target)
        {
            float diffX = target.X - this.Position.X;
            float diffY = target.Y - this.Position.Y;
            float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);

            if (distance > 0.1f) // Avoid division by zero
            {
                // Normalize and scale by speed
                float moveX = (diffX / distance) * this.Speed;
                float moveY = (diffY / distance) * this.Speed;

                this.Velocity = new Vector2(moveX, moveY);
            }
        }

    public void DebugInfo()
        {
            Console.WriteLine($"Predator {Id} - Pos: ({Position.X:F2}, {Position.Y:F2}), Energy: {Energy:F2}, Speed: {Speed:F2}, Sight: {SightRadius:F2}");
        }

    public void DetectCollisions(IEnumerable<IEntity> nearbyEntities)
        {
            foreach (IEntity entity in nearbyEntities)
            {
                if (entity != this && entity is ICollidable collidable && !collidable.IsPendingRemoval)
                {
                    float dX = entity.Position.X - this.Position.X;
                    float dY = entity.Position.Y - this.Position.Y;
                    float distSq = (dX * dX) + (dY * dY);
                    float collisionDistSq = Settings.CollisionDistance * Settings.CollisionDistance;
                    if (distSq < 0.5f) continue; // Skip if positions are exactly the same, likely a spawn issue
                    if (distSq < collisionDistSq)
                    {
                        // Simple collision response: invert velocity
                        InvertVelocityX();
                        InvertVelocityY();
                        break; // Only handle one collision per update for simplicity
                    }
                }
            }
        }

    public void ProcessStimuli(double deltaTime, IEnumerable<IEntity> nearbyEntities)
        {
            if (this.IsPendingRemoval) return;
        List<IEntity> FoodOptions = new List <IEntity>();
        IEntity closestFood = null;
        float minDistanceSq = float.MaxValue;
        float eatDistSq = Settings.PredatorEatDistance * Settings.PredatorEatDistance;
        float sightRadiusSq = SightRadius * SightRadius;
        //loop through nearby entities and eat if close enough, sense closest food otherwise
        foreach (IEntity entity in nearbyEntities) // Detect stimuli and provide suitable response
        {
            if (entity != this && !entity.IsPendingRemoval)
            {
                float dX = entity.Position.X - this.Position.X;
                float dY = entity.Position.Y - this.Position.Y;
                float distanceSq = dX * dX + dY * dY;

                if (distanceSq < eatDistSq)
                {
                    if (entity is IEatable food)
                    {
                         // Eat the food 
                        food.Consume();
                        this.Energy += food.EnergyValue; // Gain energy from eating
                    }
                   else if (entity is Critter c)
                   {
                        //take energy and kill critter
                        c.Death();
                        this.Energy += Settings.PredatorEnergyGainFromCritter;
                   }
                    else if (entity is Predator p && this.CannibalMode && p != this)
                    {
                        p.Death();
                        this.Energy += Settings.PredatorEnergyGainFromPredator;
                    }
                }
                else if (distanceSq < sightRadiusSq && distanceSq < minDistanceSq)
                {
                    bool isTarget = (entity is IEatable) ||
                    (entity is Critter) ||
                    (entity is Predator p && p != this && this.CannibalMode);
                    if (isTarget)
                    {
                        minDistanceSq = distanceSq;
                        closestFood = entity;
                    }
                }
            }
        }

       

        // DECISION PHASE
        if (closestFood != null)
        {
            SteerTowards(closestFood.Position);
        }
        else
        {
            Wander(deltaTime); // Keep moving if nothing is found
        }

        ApplyMovement(deltaTime);
        this.Energy -= _metabolism.CalculateLoss(this.Velocity,this.SightRadius, deltaTime);

        if (this.Energy >= ReproductionThreshold) 
        { 
            SpawnChild();
        }

        if (this.Energy <= 0)
        {
            this.IsPendingRemoval = true; 
            return;
        }
        }

    public void ConsumeEnergy(float amount)
        {
            this.Energy -= amount;
            if (this.Energy <= 0)
            {
                Death();
            }
        }
        


    private void Wander(double deltaTime)
        {
            // Slightly change the angle every frame for a smooth "curving" motion
            _wanderAngle += (float)(Settings.Rng.NextDouble() * 0.5 - 0.25); // Small jitter

            // Move a ALOT slower when wondering to reduce energy consumption
            float moveX = (float)Math.Cos(_wanderAngle) * (this.Speed * Settings.PredatorSpeedRatioWhenNotHungry);
            float moveY = (float)Math.Sin(_wanderAngle) * (this.Speed * Settings.PredatorSpeedRatioWhenNotHungry);

            this.Velocity = new Vector2(moveX, moveY);
        }
    public void ForcePosition(Vector2 newPos) => this.Position = newPos;
    public void Death() => this.IsPendingRemoval = true;
}