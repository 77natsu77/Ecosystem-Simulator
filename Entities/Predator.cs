//an entity which hunts critters
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.Policies;
using Ecosystem_Simulator.Core.delegates;

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

    public Predator(Vector2 startPos, PredatorGenome dna, float Energy = Settings.PredatorStartingEnergy)
    {
        this.Position = startPos;
        this._dna = dna;
        this.Energy = Energy;
        
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
        List<IEntity> FoodOptions = new List <IEntity>();
        IEntity closestFood = null;
        float minDistanceSq = float.MaxValue;
        float minDistanceSq = float.MaxValue;
        float eatDistSq = Settings.EatDistance * Settings.EatDistance;
        float sightRadiusSq = SightRadius * SightRadius;
        //loop through nearby entities and eat if close enough, sense closest food otherwise
        foreach (IEntity entity in nearbyEntities) // Detect stimuli and provide suitable response
        {
            if (!entity.IsPendingRemoval)
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
                        c.IsPendingRemoval = true;
                        this.Energy += c.Energy + Settings.PredatorEnergyGainFromCritter;
                   }
                }
                else if (distanceSq < sightRadiusSq && distanceSq < minDistanceSq)
                {
                    minDistanceSq = distanceSq;
                    closestFood = entity;
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

        

        private void Wander(double deltaTime)
        {
            // Slightly change the angle every frame for a smooth "curving" motion
            _wanderAngle += (float)(Settings.Rng.NextDouble() * 0.5 - 0.25); // Small jitter

            // Move a ALOT slower when wondering to reduce energy consumption
            float moveX = (float)Math.Cos(_wanderAngle) * (this.Speed * 0.5f);
            float moveY = (float)Math.Sin(_wanderAngle) * (this.Speed * 0.5f);

            this.Velocity = new Vector2(moveX, moveY);
        }
        public void ForcePosition(Vector2 newPos) => this.Position = newPos;
}