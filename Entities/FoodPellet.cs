using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using System.Collections.Generic;

namespace Ecosystem_Simulator.Entities
{
    public class FoodPellet : IUpdatable, IEatable, IRenderable
    {
        public Vector2 Position { get; private set; }
        public Vector2 Velocity => new Vector2(0, 0); // Food doesn't move
        public float NutritionalValue => 20f;
        public bool IsPendingRemoval { get; private set; }

        public FoodPellet(Vector2 pos)
        {
            Position = pos;
        }

        public void Update(double deltaTime, List<IEntity> nearby) { /* Do nothing */ }

        public void Consume()
        {
            // Logic to remove itself from the world
        }

        public void Draw() { /* Rendering logic later */ }

        public void InvertVelocityX()
        {

        }
        public void InvertVelocityY()
        {

        }
    }
}