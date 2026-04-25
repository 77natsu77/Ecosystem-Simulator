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
using static System.Windows.Forms.AxHost;

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
            // Setting up graphics
            System.Diagnostics.Debug.WriteLine($"Drawing {_world.Entities.Count()} entities");
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Drawing border
            using (Pen borderPen = new Pen(Color.Gray, 2f)) // 2 pixel thick line
            {
                g.DrawRectangle(borderPen, 10, 0, _world.Width, _world.Height);
            }

            // Drawing entities
            foreach (var entity in _world.Entities)
            {
                if (entity is Critter c)
                {
                    float energyRatio = Math.Max(0, Math.Min(1, (float)(c.Energy / Settings.CritterStartingEnergy)));

                    // COLOR: Smooth transition from Blue (Healthy) to Red (Starving) as energyRatio goes from 1.0 to 0.0
                    int r = (int)((1 - energyRatio) * 255);
                    int b = (int)(energyRatio * 255);
                    int gChannel = 60; // Keep a little green for depth

                    Color critterColor = Color.FromArgb(r, gChannel, b);
                    Color sightRadiusColor = Color.FromArgb(30, Color.Gray);
                    Color VelocityArrowColor = Color.FromArgb(30, Color.Gray);
                    float size = 3 + (energyRatio * 7);
                    using (Brush br = new SolidBrush(critterColor))
                    {
                        // SIZE: Gradual shrink from 10 pixels down to 3 pixels,might change these values in settings later
                        
                        // Draw Critter
                        g.FillEllipse(br, c.Position.X - size / 2, c.Position.Y - size / 2, size, size);
                        
                    }
                    using (Brush br = new SolidBrush(sightRadiusColor))
                    {
                        // Diameter is the SightRadius
                        float diameter = c.SightRadius * 2;// this fixes the visual bug for some reason...

                        // Offset is always HALF the diameter to keep it centered
                        float drawX = c.Position.X - (diameter / 2);
                        float drawY = c.Position.Y - (diameter / 2);

                        g.FillEllipse(br, drawX, drawY, diameter, diameter); 
                    }

                    // Drawing velocity arrow
                    if (Settings.DISPLAY_VELOCITY_ARROWS)
                    {
                        if (c.Velocity.X != 0 || c.Velocity.Y != 0)
                        {
                            Vector2 dir = c.Velocity / c.Speed; //Normalizing

                            float shaftLength = c.SightRadius * 2;
                            Vector2 tip = c.Position + (dir * shaftLength);

                            float headSize = 10f;
                            Vector2 baseCenter = tip - (dir * headSize); //going down from the same direction, to the triangle's base
                            Vector2 normal = new Vector2(-dir.Y, dir.X); // Rotating 90 degrees as we need the vector perpendicular to the arrow for the base

                            float halfWidth = headSize * 0.5f;
                            Vector2 leftCorner = baseCenter + (normal * halfWidth);
                            Vector2 rightCorner = baseCenter - (normal * halfWidth);
                            PointF[] headPoints = new PointF[] {
                            new PointF(tip.X, tip.Y),
                            new PointF(leftCorner.X, leftCorner.Y),
                            new PointF(rightCorner.X, rightCorner.Y)
                        };
                            g.FillPolygon(Brushes.Black, headPoints);
                            g.DrawLine(Pens.Black, c.Position.X, c.Position.Y, tip.X, tip.Y);
                        }
                    }
                   
                }
                else if (entity is Predator p) //decided that predators and critter where to similar visually and decided to change that
                {
                    float energyRatio = Math.Max(0, Math.Min(1, (float)(p.Energy / Settings.PredatorStartingEnergy)));
                    int r = (int)((1 - energyRatio) * 255);
                    int b = (int)(energyRatio * 255);
                    int gChannel = 150; // Keep a little green for depth
                    Color predatorColor = Color.FromArgb(r, gChannel, b);
                    Color sightRadiusColor = p.CannibalMode ? Color.FromArgb(30, Color.Red) : Color.FromArgb(30, Color.Black);
                    Color VelocityArrowColor = Color.FromArgb(30, Color.Gray);

                    float size = 3 + (energyRatio * 7);
                    // Define the 4 points of a diamond centered on p.Position
                    PointF[] diamondPoints = new PointF[] {
                    new PointF(p.Position.X, p.Position.Y - size),          // Top
                    new PointF(p.Position.X + size/2, p.Position.Y),       // Right
                    new PointF(p.Position.X, p.Position.Y + size),          // Bottom
                    new PointF(p.Position.X - size/2, p.Position.Y)        // Left
};
                    // Drawing predator position
                    using (Brush br = new SolidBrush(predatorColor)) // 
                    {
                        g.FillPolygon(br, diamondPoints);
                    }
                    // Drawing predator sight radius
                    using (Brush br = new SolidBrush(sightRadiusColor))
                    {
                        // Diameter is the SightRadius
                        float diameter = p.SightRadius * 2;// this fixes the visual bug for some reason...

                        // Offset is always HALF the diameter to keep it centered
                        float drawX = p.Position.X - (diameter / 2);
                        float drawY = p.Position.Y - (diameter / 2);

                        g.FillEllipse(br, drawX, drawY, diameter, diameter); 
                    }

                    // Drawing velocity arrow
                    if (Settings.DISPLAY_VELOCITY_ARROWS)
                    {
                        if (p.Velocity.X != 0 || p.Velocity.Y != 0)
                        {
                            Vector2 dir = p.Velocity / p.Speed; //Normalizing

                            float shaftLength = p.SightRadius * 2;
                            Vector2 tip = p.Position + (dir * shaftLength);

                            float headSize = 10f;
                            Vector2 baseCenter = tip - (dir * headSize); //going down from the same direction, to the triangle's base
                            Vector2 normal = new Vector2(-dir.Y, dir.X); // Rotating 90 degrees as we need the vector perpendicular to the arrow for the base

                            float halfWidth = headSize * 0.5f;
                            Vector2 leftCorner = baseCenter + (normal * halfWidth);
                            Vector2 rightCorner = baseCenter - (normal * halfWidth);
                            PointF[] headPoints = new PointF[] {
                            new PointF(tip.X, tip.Y),
                            new PointF(leftCorner.X, leftCorner.Y),
                            new PointF(rightCorner.X, rightCorner.Y)
                        };
                            g.FillPolygon(Brushes.Black, headPoints);
                            g.DrawLine(Pens.Black, p.Position.X, p.Position.Y, tip.X, tip.Y);
                        }
                    }
                    
                    // Drawing border
                    using (Pen outlinePen = new Pen(Color.Black, 2)) // 2-pixel thick black border, to make them more imposing
                    {
                        g.DrawPolygon(outlinePen, diamondPoints);
                    }

                }
                else if (entity is FoodPellet)
                {
                    // Draw Food as Green dots
                    g.FillEllipse(Brushes.LimeGreen, entity.Position.X - 2, entity.Position.Y - 2, 4, 4);
                    

                }
            }

            // Obtaining and Drawing statistics
            int critterCount = 0, foodCount = 0, predatorCount = 0;
            float sumCritterEnergy = 0, sumCritterSpeed = 0, sumCritterSight = 0, sumCritterMetab = 0, sumCritterRepro = 0;
            float sumPredatorEnergy = 0, sumPredatorSpeed = 0, sumPredatorSight = 0, sumPredatorMetab = 0, sumPredatorRepro = 0;
            foreach (var entity in _world.Entities) //ONE loop to obtain all data
            {
                if (entity is Critter c)
                {
                    critterCount++;
                    sumCritterEnergy += c.Energy;
                    sumCritterSpeed += c.Speed;
                    sumCritterSight += c.SightRadius;
                    sumCritterMetab += c.MetabolismEfficiency;
                    sumCritterRepro += c.ReproductionThreshold;
                }
                if (entity is Predator p)
                {
                    predatorCount++;
                    sumPredatorEnergy += p.Energy;
                    sumPredatorSpeed += p.Speed;
                    sumPredatorSight += p.SightRadius;
                    sumPredatorMetab += p.MetabolismEfficiency;
                    sumPredatorRepro += p.ReproductionThreshold;
                }
                else if (entity is FoodPellet)
                {
                    foodCount++;
                }
            }
            float AverageCritterEnergy = critterCount > 0 ? sumCritterEnergy / critterCount : 0;
            float AverageCritterSpeed = critterCount > 0 ? sumCritterSpeed / critterCount : 0;
            float AverageCritterSightRadius = critterCount > 0 ? sumCritterSight / critterCount : 0;
            float AverageCritterMetabolismEfficiency = critterCount > 0 ? sumCritterMetab / critterCount : 0;
            float AverageCritterReproductionThreshold = critterCount > 0 ? sumCritterRepro / critterCount : 0;

            float AveragePredatorEnergy = predatorCount > 0 ? sumPredatorEnergy / predatorCount : 0;
            float AveragePredatorSpeed = predatorCount > 0 ? sumPredatorSpeed / predatorCount : 0;
            float AveragePredatorSightRadius = predatorCount > 0 ? sumPredatorSight / predatorCount : 0;
            float AveragePredatorMetabolismEfficiency = predatorCount > 0 ? sumPredatorMetab / predatorCount : 0;
            float AveragePredatorReproductionThreshold = predatorCount > 0 ? sumPredatorRepro / predatorCount : 0;

            double lastLogTime = _historyList.Count > 0 ? _historyList.Last().Timestamp : -30; // -30 ensures it logs on frame 1
            _internalTimestamp +=Settings.TickRate;
            
            if (_internalTimestamp - lastLogTime >= Settings.StatsSaveRate)
            {
                secondsElasped += Settings.StatsSaveRate; // ensures timestamps saved are precise and consistent
                StatsEntry history = new StatsEntry
                {
                    Timestamp = secondsElasped,
                    CritterCount = critterCount,
                    FoodCount = foodCount,
                    PredatorCount = predatorCount,
                    CritterAvgSight = AverageCritterSightRadius,
                    CritterAvgEnergy = AverageCritterEnergy,
                    CritterAvgSpeed = AverageCritterSpeed,
                    CritterAvgMetabolismEfficiency = AverageCritterMetabolismEfficiency,
                    CritterAvgReproductionThreshold = AverageCritterReproductionThreshold,
                    PredatorAvgSight = AveragePredatorSightRadius,
                    PredatorAvgEnergy = AveragePredatorEnergy,
                    PredatorAvgSpeed = AveragePredatorSpeed,
                    PredatorAvgMetabolismEfficiency = AveragePredatorMetabolismEfficiency,
                    PredatorAvgReproductionThreshold = AveragePredatorReproductionThreshold
                };

                _historyList.Add(history);
                _stats_manager.SaveStatsToCSV(_historyList);
                _stats_manager.ExportToHTML(_historyList);
            }

            
            
            string Mainstats = $"\t------ ECOSYSTEM STATS ------\n" +
                           $"\n--- POPULATION DETAILS ---\n" +
                           $"Population(Critter): {critterCount}\n" +
                           $"Critter Population Status:\n" +
                           $"Population(Predator): {predatorCount}\n" +
                           $"Predator Population Status:\n" +
                           $"Food Count: {foodCount}\n" +
                           $"\n--- CRITTER DETAILS ---\n" +
                           $"Average critter speed: {AverageCritterSpeed:N1} \n" +
                           $"Average critter energy: {AverageCritterEnergy:N2} \n" +
                           $"Average critter sight radius: {AverageCritterSightRadius:N2} \n" +
                           $"Average critter metabolism efficiency: {AverageCritterMetabolismEfficiency:N5} \n" +
                           $"Average critter reproduction theshold: {AverageCritterReproductionThreshold:N0} \n" +
                           $"\n--- PREDATOR DETAILS ---\n" +
                           $"Average predator speed: {AveragePredatorSpeed:N1} \n" +
                           $"Average predator energy: {AveragePredatorEnergy:N2} \n" +
                           $"Average predator sight radius: {AveragePredatorSightRadius:N2} \n" +
                           $"Average predator metabolism efficiency: {AveragePredatorMetabolismEfficiency:N5} \n" +
                           $"Average predator reproduction theshold: {AveragePredatorReproductionThreshold:N0}";

            string critter_statusText = critterCount > 0 ? "STABLE" : "EXTINCT";
            string predator_statusText = predatorCount > 0 ? "STABLE" : "EXTINCT";
            Brush critterBrush = critterCount > 0 ? Brushes.LimeGreen : Brushes.Red;
            Brush predatorBrush = predatorCount > 0 ? Brushes.LimeGreen : Brushes.Red;
            //  Draw it to the screen
            using (Font font = new Font("Consolas", 12, FontStyle.Bold))
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(150, 0, 0, 0)), 1225, 210, 470, 450);
                // Draw the text
                g.DrawString(Mainstats, font, Brushes.White, 1250, 215);
                float lineHeight = 18.9f;
                g.DrawString(critter_statusText, font, critterBrush, 1487, 215 + (lineHeight * 4));
                g.DrawString(predator_statusText, font, predatorBrush, 1495, 215 + (lineHeight * 6));
            }

            
                
        }
    }
}
