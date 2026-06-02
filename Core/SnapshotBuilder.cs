using Ecosystem_Simulator.Environment;
using Ecosystem_Simulator.Entities;

namespace Ecosystem_Simulator.Core
{
    // This class is responsible for creating a snapshot of the world state that can be used for rendering and analysis. It will contain all the necessary information about the entities in the world, such as their positions, types, and any other relevant data that we want to include for rendering or analysis purposes.
    public class SnapshotBuilder
    {
        private readonly World _world;
        private List<EntityExportDTO> exportEntities = new List<EntityExportDTO>();
        private readonly StatisticsManager _statisticsManager;

        public SnapshotBuilder(World world, StatisticsManager statisticsManager)
        {
            _world = world;
            _statisticsManager = statisticsManager;
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

        public void CritterAddExportData(Critter c)
        {
            float eRatio = Math.Clamp(c.Energy / Settings.CritterStartingEnergy, 0, 1);
            exportEntities.Add(new EntityExportDTO {
                Id = c.Id,
                Type = "Critter", X = c.Position.X, Y = c.Position.Y,
                Size = 3 + (eRatio * 7), R = (int)((1 - eRatio) * 255), G = 60, B = (int)(eRatio * 255),
                Sight = c.SightRadius, VelX = c.Velocity.X, VelY = c.Velocity.Y, Speed = c.Speed, Energy = c.Energy
            });
        }

        public void PredatorAddExportData(Predator p)
        {
            float eRatio = Math.Clamp(p.Energy / Settings.PredatorStartingEnergy, 0, 1);
            exportEntities.Add(new EntityExportDTO {
                Id = p.Id,
                Type = "Predator", X = p.Position.X, Y = p.Position.Y,
                Size = 3 + (eRatio * 7), R = (int)(255), G = (int)((1 - eRatio) * 255), B = (int)(eRatio * 255),
                Sight = p.SightRadius, Cannibal = p.CannibalMode, VelX = p.Velocity.X, VelY = p.Velocity.Y, Speed = p.Speed, Energy = p.Energy
            });
        }

        public void SmartyAddExportData(Smarty s)
        {
            float eRatio = Math.Clamp(s.Energy / Settings.CritterStartingEnergy, 0, 1);
            exportEntities.Add(new EntityExportDTO {
                Id = s.Id,
                Type = "Smarty", X = s.Position.X, Y = s.Position.Y,
                Size = 4 + (eRatio * 8), R = (int)((1 - eRatio) * 255), G = 120, B = (int)(eRatio * 255),
                Sight = s.SightRadius, VelX = s.Velocity.X, VelY = s.Velocity.Y, Speed = s.Speed, Energy = s.Energy,
                IsScanning = s.IsScanning
            });
        }

        public void FoodAddExportData(FoodPellet f)
        {
            exportEntities.Add(new EntityExportDTO {
                Id = f.Id,
                Type = "Food", X = f.Position.X, Y = f.Position.Y, Energy = f.EnergyValue,
                Size = 4, R = 255, G = 255, B = 255
            });
        }
    }
}