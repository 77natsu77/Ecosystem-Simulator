using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Structs;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ecosystem_Simulator.Core
{
    public class StatisticsManager
    {
        private readonly string _filePath;

        public StatisticsManager(World world) // might need to rework this in the NEAR future to be more flexible and allow for recording different types of stats, but for now it just takes the world as a parameter and uses it to access the data it needs to record stats
        {
            _filePath = Settings.CSVStatsFile;
            InitializeCsv();
        }

        private void InitializeCsv()
        // will expand this in the future to include more stats, but for now it just sets up the header row for the CSV file
        {
            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                // Write the header row for the CSV file, which should match the properties of StatsEntry in the same order they are defined there to ensure correct mapping when reading the data back in for graphing and analysis
                writer.WriteLine("Timestamp,CritterCount,PredatorCount,FoodCount,CritterAvgSpeed,CritterAvgSight,CritterAvgEnergy,CritterAvgMetabolismEfficiency,CritterAvgReproductionThreshold,PredatorAvgSpeed,PredatorAvgSight,PredatorAvgEnergy,PredatorAvgMetabolismEfficiency,PredatorAvgReproductionThreshold");
            }
        }

       

        public void RecordStatistics(StatsEntry EntityInfo)
        {
            using (StreamWriter writer = new StreamWriter(_filePath, append: true))
            {
                writer.WriteLine($"{EntityInfo.Timestamp},{EntityInfo.CritterCount},{EntityInfo.PredatorCount},{EntityInfo.FoodCount},{EntityInfo.CritterAvgSpeed},{EntityInfo.CritterAvgSight},{EntityInfo.CritterAvgEnergy},{EntityInfo.CritterAvgMetabolismEfficiency},{EntityInfo.CritterAvgReproductionThreshold},{EntityInfo.PredatorAvgSpeed},{EntityInfo.PredatorAvgSight},{EntityInfo.PredatorAvgEnergy},{EntityInfo.PredatorAvgMetabolismEfficiency},{EntityInfo.PredatorAvgReproductionThreshold}");
            }
            
        }
    }
}