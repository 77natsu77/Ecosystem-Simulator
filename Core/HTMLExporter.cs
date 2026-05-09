using Ecosystem_Simulator.Core.Structs;
namespace Ecosystem_Simulator.Core
{
    public static class HTMLExporter
    {
        // will replace this with a more robust and flexible system in the future, but for now it just takes in a list of stats entries and converts them into html format for graphing and analysis, it is currently designed to work with the StatsEntry struct, but it can be easily modified to work with other types of data as well by changing the input parameter and the way the data is processed and formatted for output
        public static void ExportToHTML(List<StatsEntry> statsHistory)//chose to do this as html, wanted python but school pc prevented me from using pandas and matlib
    {
        // Convert  list of data into Javascript-friendly strings critter_
        string labels = string.Join(",", statsHistory.Select(s => $"'{s.Timestamp}s'"));
        string critterCount = string.Join(",", statsHistory.Select(s => s.CritterCount));
        string foodCount = string.Join(",", statsHistory.Select(s => s.FoodCount));
        string predatorCount = string.Join(",", statsHistory.Select(s => s.PredatorCount));
        string critter_speedData = string.Join(",", statsHistory.Select(s => s.CritterAvgSpeed));
        string critter_sight_radiusData = string.Join(",", statsHistory.Select(s => s.CritterAvgSight));
        string critter_energyData = string.Join(",", statsHistory.Select(s => s.CritterAvgEnergy));
        string critter_metabolism_efficiencyData = string.Join(",", statsHistory.Select(s => s.CritterAvgMetabolismEfficiency));
        string critter_reproduction_thresholdData = string.Join(",", statsHistory.Select(s => s.CritterAvgReproductionThreshold));
        string predator_speedData = string.Join(",", statsHistory.Select(s => s.PredatorAvgSpeed));
        string predator_sight_radiusData = string.Join(",", statsHistory.Select(s => s.PredatorAvgSight));
        string predator_energyData = string.Join(",", statsHistory.Select(s => s.PredatorAvgEnergy));
        string predator_metabolism_efficiencyData = string.Join(",", statsHistory.Select(s => s.PredatorAvgMetabolismEfficiency));
        string predator_reproduction_thresholdData = string.Join(",", statsHistory.Select(s => s.PredatorAvgReproductionThreshold));

        // Load colors from settings
        string critterPopulationColor = Settings.CritterPopulationColor;
        string foodPelletPopulationColor = Settings.FoodPelletPopulationColor;
        string predatorPopulationColor = Settings.PredatorPopulationColor;
        string speedColor = Settings.SpeedColor;
        string sightRadiusColor = Settings.SightRadiusColor;
        string energyColor = Settings.EnergyColor;
        string metabolismEfficiencyColor = Settings.MetabolismEfficiencyColor;
        string reproductionThresholdColor = Settings.ReproductionThresholdColor;
        string population_htmlTemplate = $@"
<html>
<head>
    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
</head>
<body style='background-color'>
    <h1 style='text-align: center; font-family: sans-serif;'>Ecosystem Population Stats</h1>
    <div style='width: 800px; margin: auto;'>
        <canvas id='myChart'></canvas>
    </div>
    <script>
        const ctx = document.getElementById('myChart');
        new Chart(ctx, {{
            type: 'line',
            data: {{
                labels: [{labels}],
                datasets: [{{
                    label: 'Critters',
                    data: [{critterCount}],
                    borderColor: '{critterPopulationColor}', 
                }}, {{
                    label: 'Predators',
                    data: [{predatorCount}],
                    borderColor: '{predatorPopulationColor}', 
                }}, {{
                    label: 'Food',
                    data: [{foodCount}],
                    borderColor: '{foodPelletPopulationColor}', 
                }}]
            }}
        }});
    </script>
</body>
</html>";

        File.WriteAllText(Settings.PopulationHTMLFile, population_htmlTemplate);

        string critterData_htmlTemplate = $@"
<html>
<head>
    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
</head>
<body>
    <h1 style='text-align: center; font-family: sans-serif;'>Critter Population Stats</h1>
    <div style='width: 800px; margin: auto;'>
        <canvas id='myChart'></canvas>
    </div>
    <script>
        const ctx = document.getElementById('myChart');
        new Chart(ctx, {{
            type: 'line',
            data: {{
                labels: [{labels}],
                datasets: [{{
                    label: 'Speed',
                    data: [{critter_speedData}],
                    borderColor: '{speedColor}', 
                    label: 'Sight Radius',
                    data: [{critter_sight_radiusData}],
                    borderColor: '{sightRadiusColor}', 
                }}, {{
                    label: 'Energy',
                    data: [{critter_energyData}],
                    borderColor: '{energyColor}', 
                }}, {{
                    label: 'Metabolism Efficiency',
                    data: [{critter_metabolism_efficiencyData}],
                    borderColor: '{metabolismEfficiencyColor}', 
                }}, {{
                    label: 'Reproduction Threshold',
                    data: [{critter_reproduction_thresholdData}],
                    borderColor: '{reproductionThresholdColor}', 
                }}]
            }}
        }});
    </script>
</body>
</html>";

        File.WriteAllText(Settings.CritterDataHTMLFile, critterData_htmlTemplate);

        string predatorData_htmlTemplate = $@"
<html>
<head>
    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
</head>
<body>
    <h1 style='text-align: center; font-family: sans-serif;'>Predator Population Stats</h1>
    <div style='width: 800px; margin: auto;'>
        <canvas id='myChart'></canvas>
    </div>
    <script>
        const ctx = document.getElementById('myChart');
        new Chart(ctx, {{
            type: 'line',
            data: {{
                labels: [{labels}],
                datasets: [{{
                    label: 'Speed',
                    data: [{predator_speedData}],
                    borderColor: '{speedColor}', 
                    label: 'Sight Radius',
                    data: [{predator_sight_radiusData}],
                    borderColor: '{sightRadiusColor}', 
                }}, {{
                    label: 'Energy',
                    data: [{predator_energyData}],
                    borderColor: '{energyColor}', 
                }}, {{
                    label: 'Metabolism Efficiency',
                    data: [{predator_metabolism_efficiencyData}],
                    borderColor: '{metabolismEfficiencyColor}', 
                }}, {{
                    label: 'Reproduction Threshold',
                    data: [{predator_reproduction_thresholdData}],
                    borderColor: '{reproductionThresholdColor}', 
                }}]
            }}
        }});
    </script>
</body>
</html>";

        File.WriteAllText(Settings.PredatorDataHTMLFile, predatorData_htmlTemplate);
    }
    }
}