using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.Structs;
using Ecosystem_Simulator.Entities;
using System.Collections.Generic;

namespace Ecosystem_Simulator.Environment
{

    public class World
    {
        // TODO make a new version of the current entity list as currently, iterating through each entity, which has to iterate through multiple entities as well to get neighbors, is very inefficient. We need to make a new list of entities that only contains the entities that are relevant to the current entity being updated, and then pass that list to the entity instead of the entire list of entities in the world. This will require some changes to the way we handle the spatial hash and how we store entities in the world, but it will be worth it for the performance boost. For now, we'll just have to deal with the inefficiency, but this is definitely something we need to address in the future.
        private readonly SpatialHash _grid = new SpatialHash(); // This is the spatial hash grid that we will use to efficiently find neighbors for each entity. It will divide the world into a grid of cells and keep track of which entities are in which cells, so that when we need to find neighbors for an entity, we can just look at the cells around it instead of having to iterate through every entity in the world. This should significantly improve performance, especially as the number of entities in the world grows.
        public List<IUpdatable> Entities {get; private set;} = new List<IUpdatable>(); // This is the main list of entities in the world, which will be used for updating and rendering. It will contain all the entities in the world, including critters, predators, food pellets, and any other entities we might add in the future. We will need to iterate through this list every tick to update each entity, so it is important that we keep it organized and efficient. We will also need to make sure that we update the spatial hash grid whenever we add or remove entities from this list, so that we can maintain the efficiency of our neighbor-finding logic.
        private readonly float _width;
        private readonly float _height;
       
        private List<IUpdatable> _spawnQueue = new List<IUpdatable>();
        private readonly StatisticsManager _statisticsManager;
        private SnapshotBuilder _snapshotBuilder;
        public float Width => _width;
        public float Height => _height;
        
        
        //Initializer 
        public World(float width = Settings.WorldWidth , float height = Settings.WorldHeight)
        {
            _width = width;
            _height = height;
            _critterList = new List<Critter>();
            _foodPelletList = new List<FoodPellet>();
            _statisticsManager = new StatisticsManager();
            _snapshotBuilder = new SnapshotBuilder(this, _statisticsManager);
        }


        // Event to put things into the world
        public void Spawn(IUpdatable entity)
        {
           // entity.Position = ClampToWorld(entity.Position); // Prevents spawning outside the walls
            Entities.Add(entity);
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
            /*New logic to be implemented
            -Clear export entity list at start of tick
            -for each entity, depending on the type call a specific function to add an entity to the export list
            e.g AddCritterExportData and so on
            -Then we will use the log time to check if stats nees to be saved to the csv, upon which we will calc all  the averages and such
            -frame data, which includes entity export data as well as some stats, will be returned from the snapshot and passed into the process frame function headless runner
            - the total population data will be sent to the front end by the headless runner every frame to be displayed on the screen, while the more detailed stats will be saved to the csv for later analysis. This way, we can have real-time updates on the population counts and such in the frontend without having to worry about the performance issues of calculating all the averages and such every frame, while still having access to that data for analysis after the simulation is done.
            -this new structure furthers the decoupling of the world logic from the frontend rendering, the first of many changes to come;*/
            _snapshotBuilder.exportEntities.Clear();
            foreach (IUpdatable entity in Entities)
            {
                // Save old position before update for spatial hash update later
                Vector2 oldPos = entity.Position;

                //  Get the neighbors from the grid
                IEnumerable<IEntity> neighbors = _grid.GetEntitiesInRadius(oldPos, Settings.CellSize); // Need to make a decision about wether to remove the entity itself from the neighbors list or just let the entity ignore itself in its logic. For now, we'll just let them ignore themselves, but it might be worth changing later for performance reasons.

                //  Pass them to the entity 
                entity.Update(deltaTime, neighbors);

                //Ensure they nothing clips through map
                HandleBoundaries(entity);

                //  Update the spatial registry
                _grid.UpdateEntityPosition(entity, oldPos);
            
                // add entity to export list for rendering
                // update stats 
                if (entity is Critter c)                {
                    _snapshotBuilder.CritterAddExportData(c);
                    _statisticsManager.IncrementCritterStats(c);
                }
                else if (entity is Predator p)
                {
                    _snapshotBuilder.PredatorAddExportData(p);
                    _statisticsManager.IncrementPredatorStats(p);
                }
                else if (entity is Smarty s)
                {
                    _snapshotBuilder.SmartyAddExportData(s);
                    _statisticsManager.IncrementSmartyStats(s);
                }
                else if (entity is FoodPellet f)
                {
                    _snapshotBuilder.FoodAddExportData(f);
                    _statisticsManager.IncrementFoodStats();
                }
            }

            // Cleanup Loop
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                if (Entities[i].IsPendingRemoval)
                {
                    _grid.Unregister(Entities[i]); // Tell grid to forget them
                    Entities.RemoveAt(i);          // Tell world to forget them
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

        public void Seed(int critterCount,int smartyCount, int predatorCount, int foodCount)
        {

            //  Spawn Critters
            for (int i = 0; i < critterCount; i++)
            {
                // Give each critter its OWN genome instance so they can mutate later
                CritterGenome genome = new CritterGenome();
                Vector2 pos = new Vector2(Settings.Rng.Next(0, (int)_width), Settings.Rng.Next(0, (int)_height));
                Spawn(new Critter(pos, genome));
            }
            for (int l = 0; l < smartyCount; l++){
                SmartyGenome genome = new SmartyGenome();
                Vector2 pos = new Vector2(Settings.Rng.Next(0, (int)_width), Settings.Rng.Next(0, (int)_height));
                Spawn(new Smarty(pos, genome));
            }
            // Spawn Predators
            for (int k = 0; k < predatorCount; k++)
            {
                PredatorGenome genome = new PredatorGenome();
                Vector2 pos = new Vector2(Settings.Rng.Next(0, (int)_width), Settings.Rng.Next(0, (int)_height));
                Spawn(new Predator(pos, genome));
            }
            //  Spawn Food 
            for (int j = 0; j < foodCount; j++)
            {
                Vector2 pos = new Vector2(Settings.Rng.Next(0, (int)_width), Settings.Rng.Next(0, (int)_height));
                Spawn(new FoodPellet(pos));
            }
        }
    
        // Creating a new snapshot method which runs once per tick and creates a snapshot of the world state that can be used for rendering and analysis. This way, we can decouple the simulation logic from the rendering logic and avoid any potential performance issues caused by rendering during the simulation update loop. The snapshot will contain all the necessary information about the entities in the world, such as their positions, types, and any other relevant data that we want to include for rendering or analysis purposes.
        public FrameData CreateSnapshot()
        {
            return new FrameData
            {
                Width = _world.Width,
                Height = _world.Height,
                ShowArrows = Settings.DISPLAY_VELOCITY_ARROWS,
                Stats = new PopulationStats // This is the data that gets sent to the frontend every frame to be displayed on screen
                {
                    CritterCount = _statisticsManager.CritterCount,
                    PredatorCount = _statisticsManager.PredatorCount,
                    SmartyCount = _statisticsManager.SmartyCount,
                    FoodCount = _statisticsManager.FoodCount

                    // got rid of averages every frame to save CPU, we can get that data from the stats page if needed
                },
                Entities = exportEntities
            };

        }
    
  
        
    }
}