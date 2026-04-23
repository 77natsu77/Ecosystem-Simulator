using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ecosystem_Simulator.UI
{
    public partial class Form1 : Form
    {
        private World _world;
        private System.Windows.Forms.Timer _timer;
        private List<StatsEntry> _historyList = new List<StatsEntry>();
        StatisticsManager _stats_manager = new StatisticsManager();
        private double _internalTimestamp = 0;
        private float secondsElasped = 0;
        public Form1(World world)
        {
            this._world = world;

            InitializeComponent();
            this.Paint += Form1_Paint; //  Register Paint Event
            this.DoubleBuffered = true; // Prevents flickering

            //  Setup Loop
            _timer = new System.Windows.Forms.Timer { Interval = (int)(Settings.TickRate * 1000) }; // ~60 FPS
            _timer.Tick += (s, e) => {
                _world.Tick(Settings.TickRate); // Logic Step
                this.Invalidate();  // Trigger Paint
            };
            _timer.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Drawing {_world.Entities.Count()} entities");
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Drawing border
            using (Pen borderPen = new Pen(Color.Gray, 2f)) // 2 pixel thick line
            {
                g.DrawRectangle(borderPen, 0, 0, _world.Width, _world.Height);
            }

            // Drawing entities
            foreach (var entity in _world.Entities)
            {
                if (entity is Critter c)
                {
                    float energyRatio = Math.Max(0, Math.Min(1, (float)(c.Energy / Settings.StartingEnergy)));

                    // COLOR: Smooth transition from Blue (Healthy) to Red (Starving) as energyRatio goes from 1.0 to 0.0
                    int r = (int)((1 - energyRatio) * 255);
                    int b = (int)(energyRatio * 255);
                    int gChannel = 60; // Keep a little green for depth

                    Color critterColor = Color.FromArgb(r, gChannel, b);
                    Color sightRadiusColor = Color.FromArgb(50, Color.Gray);
                    float size = 3 + (energyRatio * 7);
                    using (Brush br = new SolidBrush(critterColor))
                    {
                        // SIZE: Gradual shrink from 10 pixels down to 3 pixels
                        
                        // Draw Critter
                        g.FillEllipse(br, c.Position.X - size / 2, c.Position.Y - size / 2, size, size);
                        
                    }
                    using (Brush br = new SolidBrush(sightRadiusColor))
                    {
                        // Diameter is the SightRadius
                        float diameter = c.SightRadius;

                        // Offset is always HALF the diameter to keep it centered
                        float drawX = c.Position.X - (diameter / 2);
                        float drawY = c.Position.Y - (diameter / 2);

                        g.FillEllipse(br, drawX, drawY, diameter, diameter);
                    }
                }
                else if (entity is FoodPellet)
                {
                    // Draw Food as Green dots
                    g.FillEllipse(Brushes.LimeGreen, entity.Position.X - 2, entity.Position.Y - 2, 4, 4);
                }
            }

            // Obtaining and Drawing statistics
            int critterCount = 0;
            int foodCount = 0;
            float sumEnergy = 0, sumSpeed = 0, sumSight = 0, sumMetab = 0, sumRepro = 0;
            foreach (var entity in _world.Entities) //ONE loop to obtain all data
            {
                if (entity is Critter c)
                {
                    critterCount++;
                    sumEnergy += c.Energy;
                    sumSpeed += c.Speed;
                    sumSight += c.SightRadius;
                    sumMetab += c.MetabolismEfficiency;
                    sumRepro += c.ReproductionThreshold;
                }
                else if (entity is FoodPellet)
                {
                    foodCount++;
                }
            }
            float AverageEnergy = critterCount > 0 ? sumEnergy / critterCount : 0;
            float AverageSpeed = critterCount > 0 ? sumSpeed / critterCount : 0;
            float AverageSightRadius = critterCount > 0 ? sumSight / critterCount : 0;
            float AverageMetabolismEfficiency = critterCount > 0 ? sumMetab / critterCount : 0;
            float AverageReproductionThreshold = critterCount > 0 ? sumRepro / critterCount : 0;
            //TODO: GET RID OF DECIMALS APPEARING IN HTML FILES (THINKING OF A SECONDS ELASPED SYSTEM USING THE NO OF FRAMES AND MOD 60,NEED A WAY TO CONVERT fps TO SECONDS AS WELL IN SETTINGS)
            double lastLogTime = _historyList.Count > 0 ? _historyList.Last().Timestamp : -30; // -30 ensures it logs on frame 1
            _internalTimestamp +=Settings.TickRate;
            
            if (_internalTimestamp - lastLogTime >= Settings.StatsSaveRate)
            {
                secondsElasped += Settings.StatsSaveRate; //trying this instead so the html files look cleaner
                StatsEntry history = new StatsEntry
                {
                    Timestamp = secondsElasped,
                    CritterCount = critterCount,
                    FoodCount = foodCount,
                    AvgSight = AverageSightRadius,
                    AvgEnergy = AverageEnergy,
                    AvgSpeed = AverageSpeed,
                    AvgMetabolismEfficiency = AverageMetabolismEfficiency,
                    AvgReproductionThreshold = AverageReproductionThreshold
                };

                _historyList.Add(history);
                _stats_manager.SaveStatsToCSV(_historyList);
                _stats_manager.ExportToHTML(_historyList);
            }

            
            
            string stats = $"--- ECOSYSTEM STATS ---\n" +
                           $"Population: {critterCount}\n" +
                           $"Food Count: {foodCount}\n" +
                           $"Average speed: {AverageSpeed} \n" +
                           $"Average energy: {AverageEnergy:N2} \n" +
                           $"Average sight radius: {AverageSightRadius} \n" +
                           $"Average metabolism efficiency: {AverageMetabolismEfficiency:N5} \n" +
                           $"Average reproduction theshold: {AverageReproductionThreshold:N0} \n" +
                           $"Status: {(critterCount > 0 ? "Stable" : "EXTINCT")}";

            //  Draw it to the screen
            using (Font font = new Font("Consolas", 12, FontStyle.Bold))
            {
                // Draw a small background box for readability
                g.FillRectangle(new SolidBrush(Color.FromArgb(150, 0, 0, 0)), 10, 10, 370, 200);
                // Draw the text
                g.DrawString(stats, font, Brushes.White, 15, 15);
            }

            
                
        }
    }
}
