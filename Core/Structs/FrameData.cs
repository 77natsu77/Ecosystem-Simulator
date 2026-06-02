//A class to hold the data for a single frame of animation, including the position and rotation of each bone in the skeleton.
using System;
namespace Ecosystem_Simulator.Core.Structs
{   
    public struct FrameData
    {
        float WorldWidth;
        float WorldHeight;
        bool showArros;
        PopulationStats stats;
        List<EntityExportDTO> exportEntities;
        public FrameData(float worldWidth, float worldHeight, bool showArrows, PopulationStats stats, List<EntityExportDTO> exportEntities)
        {
            WorldWidth = worldWidth;
            WorldHeight = worldHeight;
            this.showArros = showArrows;
            this.stats = stats;
            this.exportEntities = exportEntities;
        }
    }
}