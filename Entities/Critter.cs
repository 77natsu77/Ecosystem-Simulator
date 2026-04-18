using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using System;
using System.Collections.Generic;
namespace Ecosystem_Simulator.Entities
{
    public class Critter : IUpdatable
    {
        private readonly IEnergyPolicy _metabolism;
        private readonly IGenome _dna;

        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float Energy { get; private set; }

        public Critter(Vector2 startPos, IEnergyPolicy policy, IGenome dna)
        {
            Position = startPos;
            _metabolism = policy;
            _dna = dna;
            Energy = 100f;
        }

        public void Update(double deltaTime, List<IUpdatable> nearbyEntities)
        {
            //  Record the position BEFORE moving
            Vector2 oldPos = this.Position;
            Vector2 newPos = new Vector2();

            //  Perform Movement
            //  NewPos = OldPos + (Velocity * deltaTime)
            newPos.X = (float)(oldPos.X + (this.Velocity.X * deltaTime));
            newPos.Y = (float)(oldPos.Y + (this.Velocity.Y * deltaTime));
            this.Position = newPos;

            //  Update the Grid
            // We need a reference to the SpatialHash to tell it we moved
            // Should the Critter have a reference to the Hash, or should the World handle this?

            //sensing food
            foreach (IUpdatable entity in nearbyEntities)
            {
                if (entity is IEatable food)
                {

                }
            }

            //  Energy Consumption
             float cost = _metabolism.CalculateLoss(Velocity, deltaTime);
            this.Energy -= cost;
        }
    }
}