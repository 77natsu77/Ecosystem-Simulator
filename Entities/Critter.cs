using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
namespace Ecosystem_Simulator.Entities
{
    public class Critter : IUpdatable, IMovable
    {
        private readonly IEnergyPolicy _metabolism;
        private readonly IGenome _dna;

        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float Energy { get; private set; }
        public bool IsPendingRemoval { get; private set; }

        public Critter(Vector2 startPos, IEnergyPolicy policy, IGenome dna)
        {
            Position = startPos;
            _metabolism = policy;
            _dna = dna;
            Energy = 100f;
        }

        public void Update(double deltaTime, List<IEntity> nearbyEntities)
        {
            //  Record the position BEFORE moving
            Vector2 oldPos = this.Position;
            Vector2 newPos = new Vector2();

            //  Perform Movement
            //  NewPos = OldPos + (Velocity * deltaTime)
            newPos.X = (float)(oldPos.X + (this.Velocity.X * deltaTime));
            newPos.Y = (float)(oldPos.Y + (this.Velocity.Y * deltaTime));
            this.Position = newPos;


            //sensing food
            foreach (IEntity entity in nearbyEntities)
            {
                if (entity is IEatable food)
                {
                    float dist = CalculateDistance(this.Position, entity.Position);

                    if (dist < (Settings.EatDistance * Settings.EatDistance)) // Close enough to eat
                    {
                        this.Energy += food.NutritionalValue;
                        food.Consume(); // This should set its 'IsExpired' to true
                    }
                }
            }

            // Inside Critter.Update
            bool isHungry = this.Energy < (Settings.StartingEnergy * 0.5f);
            
            if (isHungry)
            {
                // 1. Find Closest Food
                // 2. Set Velocity toward it (Seek behavior)
            }
            else
            {
                // 1. Wander aimlessly (Save energy)
                // 2. Or look for a mate? (Phase 2)
            }

            //  Energy Consumption
            float cost = _metabolism.CalculateLoss(Velocity, deltaTime);
            this.Energy -= cost;
        }

        public float CalculateDistance(Vector2 A, Vector2 B)
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
        }
        public void InvertVelocityY()
        {
            Vector2 newVelocity = new Vector2();
            newVelocity.X = this.Velocity.X;
            newVelocity.Y = -this.Velocity.Y;
            this.Velocity = newVelocity;
        }

        public void ForcePosition(Vector2 newPos) => this.Position = newPos;
    }
}