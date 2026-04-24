using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class StatisticsManager
{
    public void SaveStatsToCSV(List<StatsEntry> statsHistory)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Timestamp,CritterCount,FoodCount,AvgSight,AvgEnergy,AvgSpeed");
        foreach (var entry in statsHistory)
        {
            sb.AppendLine($"{entry.Timestamp},{entry.CritterCount},{entry.FoodCount},{entry.AvgSight},{entry.AvgEnergy},{entry.AvgSpeed}");
        }
        File.WriteAllText(Settings.StatsFilePath, sb.ToString());
    }



    public void SaveWorld(string path, World world)
    {
        var data = new SaveData(world);

        // Pack the data
        foreach (var entity in world.Entities)
        {
            if (entity is Critter c)
            {
                data.Critters.Add(new CritterSaveData
                {
                    Position = c.Position,
                    Energy = c.Energy,
                    Speed = c.Speed, // Assuming access to genome
                    SightRadius = c.SightRadius,
                    MetabolismEfficiency = c.MetabolismEfficiency,
                    ReproductionThreshold = c.ReproductionThreshold
                });
            }
            else if (entity is FoodPellet f)
            {
                data.FoodPositions.Add(f.Position);
            }
        }
        //TODO IMPLEMENT JSON, system.Text.Json seems to not work for some reason, maybe because v4.7.2
        //string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        // File.WriteAllText(path, json);
    }

    public void SaveWorldManual(World world)//json doesnt work on version 4.7.2, only 5 and above (I think)
    {
        var data = new SaveData(world);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{"); // Open file
        sb.AppendLine($"  \"WorldWidth\": {data.WorldWidth},");
        sb.AppendLine($"  \"WorldHeight\": {data.WorldHeight},");

        sb.AppendLine("  \"Critters\": ["); // Open critter list
        for (int i = 0; i < data.Critters.Count; i++)
        {
            var c = data.Critters[i];
            sb.AppendLine("    {");
            sb.AppendLine($"      \"Energy\": {c.Energy},");
            sb.AppendLine($"      \"Position\": {{ \"X\": {c.Position.X}, \"Y\": {c.Position.Y} }},");
            sb.AppendLine($"      \"Speed\": {c.Speed},");
            sb.AppendLine($"      \"SightRadius\": {c.SightRadius},");
            sb.AppendLine($"      \"MetabolismEfficiency\": {c.MetabolismEfficiency},");
            sb.AppendLine($"      \"ReproductionThreshold\": {c.ReproductionThreshold}");
            sb.Append("    }");
            if (i < data.Critters.Count - 1) sb.Append(","); // JSON requires commas between items
            sb.AppendLine();
        }
        sb.AppendLine("  ],"); // Close critter list

        sb.AppendLine("  \"FoodPositions\": ["); // Open food position list
        for (int i = 0; i < data.FoodPositions.Count; i++)
        {
            var f = data.FoodPositions[i];
            sb.AppendLine($"    {{ \"X\": {f.X}, \"Y\": {f.Y} }}");
            if (i < data.FoodPositions.Count - 1) sb.Append(",");
            sb.AppendLine();
        }
        sb.AppendLine("  ]"); // Close food position list
        sb.AppendLine("}"); // Close file
        File.WriteAllText(Settings.WorldSaveFile, sb.ToString());
    }

    public void LoadFromData(SaveData data, World world)//NEED A WAY TO CONVERT FROM STRING TO SAVEDATA FORMAT
    {
        foreach (var dto in data.Critters)
        {
            var genome = new DefaultGenome(dto.Speed, dto.SightRadius, dto.MetabolismEfficiency, dto.ReproductionThreshold);
            var critter = new Critter(dto.Position, genome, dto.Energy);
            world.Spawn(critter);
        }

        foreach (var foodPos in data.FoodPositions)
        {
            world.Spawn(new FoodPellet(foodPos));
        }
    }

    public SaveData LoadFromJson(string json)
    {
        // Currently stubbed - need to parse manually
        return null;
    }

    public void ExportToHTML(List<StatsEntry> statsHistory)//chose to do this as html, wanted python but school pc prevented me from using pandas and matlib
    {
        // Convert  list of data into Javascript-friendly strings
        string labels = string.Join(",", statsHistory.Select(s => $"'{s.Timestamp}s'"));
        string critterCount = string.Join(",", statsHistory.Select(s => s.CritterCount));
        string foodCount = string.Join(",", statsHistory.Select(s => s.FoodCount));
        string speedData = string.Join(",", statsHistory.Select(s => s.AvgSpeed));
        string sight_radiusData = string.Join(",", statsHistory.Select(s => s.AvgSight));
        string energyData = string.Join(",", statsHistory.Select(s => s.AvgEnergy));
        string metabolism_efficiencyData = string.Join(",", statsHistory.Select(s => s.AvgMetabolismEfficiency));
        string reproduction_thresholdData = string.Join(",", statsHistory.Select(s => s.AvgReproductionThreshold));

        // Little addition, change background color if extinction occurs, just to make it more visually striking, also makes it easier to see the graphs if there are only a few critters left at the end
        bool isExtinct = statsHistory.Last().CritterCount == 0;
        string bgColor = isExtinct ? "#440000" : "#ffffff";

        // Load colors from settings
        string critterPopulationColor = Settings.CritterPopulationColor;
        string foodPelletPopulationColor = Settings.FoodPelletPopulationColor;
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
    <body style='background-color: {bgColor};'>
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
                        borderColor: {critterPopulationColor},
                    }}, {{
                        label: 'Food',
                        data: [{foodCount}],
                        borderColor: {foodPelletPopulationColor},
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
                        data: [{speedData}],
                        borderColor: {speedColor},
                    }}, {{
                        label: 'Sight Radius',
                        data: [{sight_radiusData}],
                        borderColor: {sightRadiusColor},
                    }}, {{
                        label: 'Energy',
                        data: [{energyData}],
                        borderColor: {energyColor},
                    }}, {{
                        label: 'Metabolism Efficiency',
                        data: [{metabolism_efficiencyData}],
                        borderColor: {metabolismEfficiencyColor},
                    }}, {{
                        label: 'Reproduction Threshold',
                        data: [{reproduction_thresholdData}],
                        borderColor: {reproductionThresholdColor},
                        }}]
                }}
            }});
        </script>
    </body>
    </html>";
        File.WriteAllText(Settings.CritterDataHTMLFile, critterData_htmlTemplate);
    }
   
}


public class SaveData
{
    public float WorldWidth { get; set; }
    public float WorldHeight { get; set; }
    public List<CritterSaveData> Critters { get; set; } = new List<CritterSaveData>();
    public List<Vector2> FoodPositions { get; set; } = new List<Vector2>();

    public SaveData(World world)
    {
        WorldWidth = world.Width;
        WorldHeight = world.Height;
        foreach (var entity in world.Entities)
        {
            if (entity is Critter c)
            {
                Critters.Add(new CritterSaveData
                {
                    Position = c.Position,
                    Energy = c.Energy,
                    Speed = c.Speed,
                    SightRadius = c.SightRadius,
                    MetabolismEfficiency = c.MetabolismEfficiency,
                    ReproductionThreshold = c.ReproductionThreshold
                });
            }
            else if (entity is FoodPellet f)
            {
                FoodPositions.Add(f.Position);
            }
        }
    }
}

public class CritterSaveData //Need to get back to this
{
    public Vector2 Position { get; set; }
    public float Energy { get; set; }
    public float Speed { get; set; }
    public float SightRadius { get; set; }
    public float MetabolismEfficiency { get; set; }
    public float ReproductionThreshold { get; set; }

}