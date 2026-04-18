namespace Ecosystem_Simulator.Environment
{
    using Ecosystem_Simulator.Core;
    using Ecosystem_Simulator.Core.Interfaces;
    using System.Collections.Generic;

    public class World
    {
        private readonly SpatialHash _grid = new SpatialHash();

        // This holds every active entity in the simulation
        private readonly List<IUpdatable> _entities = new List<IUpdatable>();

        // We need a way to put things into the world
        public void Spawn(IUpdatable entity)
        {
            _entities.Add(entity);
            _grid.Register(entity);
        }

        public void Tick(double deltaTime)
        {
            foreach (IUpdatable entity in _entities)
            {
                Vector2 oldPos = entity.Position;

                //  Get the neighbors from the grid
                List<IUpdatable> neighbors = _grid.GetNearby(entity.Position);

                //  Pass them to the entity 
                entity.Update(deltaTime, neighbors);

                //  Update the spatial registry
                _grid.UpdateEntityPosition(entity, oldPos);

                //TODO: Death handling if energy = 0
            }
        }
    }
}