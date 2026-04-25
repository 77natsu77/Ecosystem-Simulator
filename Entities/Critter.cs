using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.delegates;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.Policies;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
namespace Ecosystem_Simulator.Entities
{
    public class Critter : IUpdatable, IMovable
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

        public CritterGenome DNA => (CritterGenome)_dna;

        public event SpawnRequestDelegate OnSpawnRequested;

        public Critter(Vector2 startPos, CritterGenome dna, float Energy = Settings.CritterStartingEnergy)
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
            if (this.IsPendingRemoval) return;
            IEatable closestFood = null;
            float minDistanceSq = float.MaxValue;
            float eatDistSq = Settings.CritterEatDistance * Settings.CritterEatDistance;
            float sightRadiusSq = SightRadius * SightRadius;

            foreach (IEntity entity in nearbyEntities) // Detect stimuli and provide suitable response
            {
                if (entity is IEatable food && !food.IsPendingRemoval)
                {
                    float dX = entity.Position.X - this.Position.X;
                    float dY = entity.Position.Y - this.Position.Y;
                    float distSq = (dX * dX) + (dY * dY);
                    // Very complex logic incoming, brace yourself
                    // ACTION 1: EATING
                    if (distSq < eatDistSq)
                    {
                        this.Energy += food.EnergyValue;
                        food.Consume();
                        continue; // Move to next entity, this one is gone
                    }

                    // ACTION 2: SENSING FOOD (Only if hungry)
                    if (this.Energy < Settings.CritterHungerEnergy) 
                    {
                        if (distSq < sightRadiusSq && distSq < minDistanceSq)
                        {
                            minDistanceSq = distSq;
                            closestFood = food;
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
                Death();
                return;
            }

           
        }

        public void SpawnChild()
        {
            float baby_energy = this.Energy * Settings.CritterBirthEnergyShareRatio;
            this.Energy -= baby_energy; //check if multiplying by 1-ratio is faster, need to make things more efficienct for heavier tests
            
            // Create new critter at parent position
            Critter baby = new Critter(this.Position, new CritterGenome(this.Speed,this.SightRadius,this.MetabolismEfficiency,this.ReproductionThreshold,true),baby_energy );

            //  Trigger spawn event
            OnSpawnRequested?.Invoke(baby);
        }



        public float CalculateDistance(Vector2 A, Vector2 B)// Move this into vector struct?
        {
            float diffX = A.X - B.X;
            float diffY = A.Y - B.Y;
            return diffX * diffX + diffY * diffY;
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

            // Move a bit slower when wondering to reduce energy consumption
            float moveX = (float)Math.Cos(_wanderAngle) * (this.Speed * Settings.CritterSpeedRatioWhenNotHungry);
            float moveY = (float)Math.Sin(_wanderAngle) * (this.Speed * Settings.CritterSpeedRatioWhenNotHungry);

            this.Velocity = new Vector2(moveX, moveY);
        }
        public void ForcePosition(Vector2 newPos) => this.Position = newPos;
        public void Death() => this.IsPendingRemoval = true;
    }
}