using Ecosystem_Simulator.Environment;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Core.Structs;
using System.Text.Json;
namespace Ecosystem_Simulator.Core.Saving
{
    public class WorldPersistence
    {
        public void SaveWorldState(World world, string filePath)
        {
            SaveWorldData data = new SaveWorldData();
            data.WorldWidth = world.Width;
            data.WorldHeight = world.Height;
            // Implement logic to serialize the world state and save it to a file
            foreach (var entity in world.Entities)
            {
                if (entity is Critter c)
                {
                    // Add critterData to a list for saving
                    data.Animnals.Add(new EntityExportDTO
                    {
                        Type = "Critter",
                        X = c.Position.X,
                        Y = c.Position.Y,
                        Sight = c.SightRadius,
                        VelX = c.Velocity.X,
                        VelY = c.Velocity.Y,
                        Speed = c.Speed,
                        Energy = c.Energy,
                        Id = c.Id

                    });
                }
                else if (entity is Predator p)
                {
                    // Add predatorData to a list for saving
                    data.Animnals.Add(new EntityExportDTO
                    {
                        Type = "Predator",
                        X = p.Position.X,
                        Y = p.Position.Y,
                        Sight = p.SightRadius,
                        VelX = p.Velocity.X,
                        VelY = p.Velocity.Y,
                        Speed = p.Speed,
                        Energy = p.Energy,
                        Cannibal = p.CannibalMode,
                        Id = p.Id
                    });
                }
                else if (entity is FoodPellet f)
                {
                    // Save food pellet data
                    var foodData = new 
                    {
                        Position = f.Position
                    };
                    // Add foodData to a list for saving
                    data.FoodPositions.Add(foodData.Position);
                }
            }

            //string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }); 
            File.WriteAllText(filePath, json);
        }

        public World LoadWorldState(string filePath)
        {
            // Implement logic to read the world state from a file and deserialize it into a World object
            
            string json = File.ReadAllText(filePath);
            SaveWorldData data = JsonSerializer.Deserialize<SaveWorldData>(json);
            if (data == null) throw new Exception("Failed to deserialize world data.");
            World newWorld = new World(data.WorldWidth, data.WorldHeight);
            foreach (var animalData in data.Animnals)
            {
                if (animalData.Type == "Predator")
                {
                    Predator predator = new Predator(new Vector2(animalData.X, animalData.Y), new PredatorGenome(animalData.Speed, animalData.Sight, animalData.MetabolismEfficiency, animalData.ReproductionThreshold, true), animalData.Energy);
                    newWorld.Spawn(predator);
                }
                else
                {
                    Critter critter = new Critter(new Vector2(animalData.X, animalData.Y), new CritterGenome(animalData.Speed, animalData.Sight, animalData.MetabolismEfficiency, animalData.ReproductionThreshold), animalData.Energy);
                    newWorld.Spawn(critter);
                }
            }
            foreach (var foodPos in data.FoodPositions)
            {
                newWorld.Spawn(new FoodPellet(foodPos));
            }
            return newWorld;
        }
        
    }

    public class SaveWorldData // could make this a struct if we want, but it is not really performance critical and it is easier to work with as a class, especially when deserializing from json, so I will leave it as a class for now
    {
        public float WorldWidth { get; set; }
        public float WorldHeight { get; set; }
        public List<EntityExportDTO> Animnals { get; set; } = new List<EntityExportDTO>();
        // Will need to change this if  more complex food types are added, but for now it is just a list of positions
        public List<Vector2> FoodPositions { get; set; } = new List<Vector2>();
    }


}