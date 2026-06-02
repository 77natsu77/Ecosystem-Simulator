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
        private double _internalTimestamp = 0;
        private double lastLogTime = -30; // Initialize to -30 to ensure it logs on the first frame (assuming StatsSaveRate is 30 seconds or less)
        private float _secondsElapsed = 0;
        private List<StatsEntry> _historyList = new List<StatsEntry>(); // This list will store the history of all stats entries, which can be used for both CSV and HTML exports. This way we keep a complete history of all the stats entries, and we can optimize the HTML export later to only update the latest entry instead of rewriting the whole file every time.
        private EntityStats _currentStats = new EntityStats(); // This struct will hold the current stats for the current frame, which will be updated every frame and then used to create a new StatsEntry when it's time to log the data. This way we can keep track of the current stats without having to recalculate them from scratch every time we want to log the data, which should improve performance.
        private bool _isTimeToLog => _internalTimestamp - lastLogTime >= Settings.StatsSaveRate; // This flag will be set to true when it's time to log the data, which will trigger the logging process in the CalculateStatistics method. This way we can separate the logic for updating the current stats from the logic for logging the data, which should make the code cleaner and easier to maintain.
        public int CritterCount => _currentStats.CritterCount;
        public int PredatorCount => _currentStats.PredatorCount;
        public int SmartyCount => _currentStats.SmartyCount;
        public int FoodCount => _currentStats.FoodCount;

        public void CalculateStatistics()
        {
            double lastLogTime = _historyList.Count > 0 ? _historyList.Last().Timestamp : -30; // -30 ensures it logs on frame 1
            _internalTimestamp += Settings.TickRate;

            if (_isTimeToLog) // Save data every StatsSaveRate seconds, independent of TickRate (which can be changed for performance reasons and doesn't need to affect the stats logging)
            {
                _secondsElapsed += Settings.StatsSaveRate; // ensures timestamps saved are precise and consistent
                StatsEntry EntityInfo = new StatsEntry // data for graphs and csv files
                {
                    Timestamp = _secondsElapsed,
                    CritterCount = _currentStats.CritterCount,
                    FoodCount = _currentStats.FoodCount,
                    PredatorCount = _currentStats.PredatorCount,
                    SmartyCount = _currentStats.SmartyCount,
                    CritterAvgSight = _currentStats.AverageCritterSightRadius,
                    CritterAvgEnergy = _currentStats.AverageCritterEnergy,
                    CritterAvgSpeed = _currentStats.AverageCritterSpeed,
                    CritterAvgMetabolismEfficiency = _currentStats.AverageCritterMetabolismEfficiency,
                    CritterAvgReproductionThreshold = _currentStats.AverageCritterReproductionThreshold,
                    PredatorAvgSight = _currentStats.AveragePredatorSightRadius,
                    PredatorAvgEnergy = _currentStats.AveragePredatorEnergy,
                    PredatorAvgSpeed = _currentStats.AveragePredatorSpeed,
                    PredatorAvgMetabolismEfficiency = _currentStats.AveragePredatorMetabolismEfficiency,
                    PredatorAvgReproductionThreshold = _currentStats.AveragePredatorReproductionThreshold
                };

                _historyList.Add(EntityInfo); // Add the new stats entry to the history list, which is used for both CSV and HTML exports. This way we keep a complete history of all the stats entries, and we can optimize the HTML export later to only update the latest entry instead of rewriting the whole file every time.
                RecordStatistics(EntityInfo);
                HTMLExporter.ExportToHTML(_historyList); // Needs work, currently it just overwrites the same html file every time with the full history, which is inefficient and causes performance issues as the history grows. We need to either optimize the HTMLExporter to only update the data in the existing file without rewriting the whole thing, or we need to implement a different system for storing and accessing the stats data for graphing (like a lightweight database or in-memory data structure that the frontend can query). For now, we'll just rely on the CSV file for stats analysis and leave the HTMLExporter as a future improvement.
            } 
        }

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

        public void RecordStatistics(StatsEntry EntityInfo) //might make this private now sinsce it is only used in this class
        {
            using (StreamWriter writer = new StreamWriter(_filePath, append: true))
            {
                writer.WriteLine($"{EntityInfo.Timestamp},{EntityInfo.CritterCount},{EntityInfo.PredatorCount},{EntityInfo.FoodCount},{EntityInfo.CritterAvgSpeed},{EntityInfo.CritterAvgSight},{EntityInfo.CritterAvgEnergy},{EntityInfo.CritterAvgMetabolismEfficiency},{EntityInfo.CritterAvgReproductionThreshold},{EntityInfo.PredatorAvgSpeed},{EntityInfo.PredatorAvgSight},{EntityInfo.PredatorAvgEnergy},{EntityInfo.PredatorAvgMetabolismEfficiency},{EntityInfo.PredatorAvgReproductionThreshold}");
            }
            
        }
    
        public void IncrementCritterStats(Critter c)
        {
            if (_isTimeToLog)
            {
                 _currentStats.IncrementCritterStats(c.Energy, c.Speed, c.SightRadius, c.MetabolismEfficiency, c.ReproductionThreshold); // Update the current stats with the new critter's data. This way we can keep track of the current stats without having to recalculate them from scratch every time we want to log the data, which should improve performance. We only update the current stats when it's time to log, which ensures that the stats are accurate and up-to-date when we create a new StatsEntry for logging.
            }
            else
            {
                _currentStats.IncrementCritterStats(); // Increase population only
            }
        }
        public void IncrementPredatorStats(Predator p)
        {
           if (_isTimeToLog)
            {
                _currentStats.IncrementPredatorStats(p.Energy, p.Speed, p.SightRadius, p.MetabolismEfficiency, p.ReproductionThreshold, p.PathUpdateInterval); // Update the current stats with the new predator's data. This way we can keep track of the current stats without having to recalculate them from scratch every time we want to log the data, which should improve performance. We only update the current stats when it's time to log, which ensures that the stats are accurate and up-to-date when we create a new StatsEntry for logging.
            }
            else
            {
                _currentStats.IncrementPredatorStats(); // Increase population only
            }
        }
        public void IncrementSmartyStats(Smarty s)
        {
            if (_isTimeToLog)
            {
                _currentStats.IncrementSmartyStats(s.Energy, s.Speed, s.SightRadius, s.MetabolismEfficiency, s.ReproductionThreshold, s.PathUpdateInterval); // Update the current stats with the new smarty’s data. This way we can keep track of the current stats without having to recalculate them from scratch every time we want to log the data, which should improve performance. We only update the current stats when it's time to log, which ensures that the stats are accurate and up-to-date when we create a new StatsEntry for logging.
            }
            else
            {
                _currentStats.IncrementSmartyStats(); // Increase population only
            }
        }
        public void IncrementFoodStats()
        {
            _currentStats.IncrementFoodStats(); // increase population only, since food doesn't have any other stats to track for now, but we can easily expand this in the future if we want to track different types of food with different properties (like energy value, respawn rate, etc.) by adding parameters to this method and updating the EntityStats struct accordingly.
        }
    }
}