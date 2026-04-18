namespace Ecosystem_Simulator.Environment
{
    using Ecosystem_Simulator.Core;
    using Ecosystem_Simulator.Core.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class World
    {
        private readonly SpatialHash _grid = new SpatialHash();

        // This holds every active entity in the simulation
        private readonly List<IUpdatable> _entities = new List<IUpdatable>();
        private readonly float _width;
        private readonly float _height;
        private List<IUpdatable> _spawnQueue = new List<IUpdatable>();

        //Initializer 
        public World(float width, float height)
        {
            _width = width;
            _height = height;
        }

        // We need a way to put things into the world
        public void Spawn(IUpdatable entity)
        {
            _entities.Add(entity);
            _grid.Register(entity);

            // If the entity is capable of requesting spawns, listen to it!
            if (entity is IUpdatable updatable)
            {
                // Using += "connects" the entity's event to the World's RequestSpawn method
                updatable.OnSpawnRequested += this.RequestSpawn;
            }
        }

        public void RequestSpawn(IEntity entity)
        {
            if (entity is IUpdatable updatable)
            {
                _spawnQueue.Add(updatable);
            }           
        }

        public void Tick(double deltaTime)
        {
            foreach (IUpdatable entity in _entities)
            {
                Vector2 oldPos = entity.Position;

                //  Get the neighbors from the grid
                IEnumerable<IEntity> neighbors = _grid.GetEntitiesInRadius(oldPos,Settings.CellSize);

                //  Pass them to the entity 
                entity.Update(deltaTime, neighbors);

                //Ensure they nothing clips through map
                HandleBoundaries(entity);

                //  Update the spatial registry
                _grid.UpdateEntityPosition(entity, oldPos);
            }

            // Cleanup Loop
            for (int i = _entities.Count - 1; i >= 0; i--)
            {
                if (_entities[i].IsPendingRemoval)
                {
                    _grid.Unregister(_entities[i]); // Tell grid to forget them
                    _entities.RemoveAt(i);          // Tell world to forget them
                }
            }
            //Final step: process spawn queue
            if (_spawnQueue.Count > 0)
            {
                foreach (var baby in _spawnQueue)
                {
                    Spawn(baby);
                }
                _spawnQueue.Clear();
            }
        }

        private void HandleBoundaries(IEntity entity)
        {
            // We only care about boundaries for things that actually move!
            if (entity is IMovable movable)
            {
                float x = movable.Position.X;
                float y = movable.Position.Y;
                bool bounced = false;

                if (x < 0) { x = 0; movable.InvertVelocityX(); bounced = true; }
                else if (x > _width) { x = _width; movable.InvertVelocityX(); bounced = true; }

                if (y < 0) { y = 0; movable.InvertVelocityY(); bounced = true; }
                else if (y > _height) { y = _height; movable.InvertVelocityY(); bounced = true; }

                if (bounced)
                {
                    movable.ForcePosition(new Vector2(x, y));
                }
            }
        }
    }
}