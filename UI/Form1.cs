using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ecosystem_Simulator.UI
{
    public partial class Form1 : Form
    {
        private World _world;
        private System.Windows.Forms.Timer _timer;

        public Form1()
        {
            InitializeComponent();
            this.Paint += Form1_Paint; //  Register Paint Event
            this.DoubleBuffered = true; // Prevents flickering

            // Initialize world
            _world = new World(this.ClientSize.Width, this.ClientSize.Height);
            _world.Seed(Settings.InitialCritterNumber, Settings.InitialFoodPelletNumber); 

            //  Setup Loop
            _timer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 FPS
            _timer.Tick += (s, e) => {
                _world.Tick(0.016); // Logic Step
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

                    // COLOR: Smooth transition from Blue (Healthy) to Red (Starving)
                    // As energyRatio goes from 1.0 to 0.0
                    int r = (int)((1 - energyRatio) * 255);
                    int b = (int)(energyRatio * 255);
                    int gChannel = 60; // Keep a little green for depth

                    Color critterColor = Color.FromArgb(r, gChannel, b);

                    using (Brush br = new SolidBrush(critterColor))
                    {
                        // SIZE: Gradual shrink from 10 pixels down to 3 pixels
                        float size = 3 + (energyRatio * 7);

                        g.FillEllipse(br, c.Position.X - size / 2, c.Position.Y - size / 2, size, size);
                    }
                }
                else if (entity is FoodPellet)
                {
                    // Draw Food as Green dots
                    g.FillEllipse(Brushes.LimeGreen, entity.Position.X - 2, entity.Position.Y - 2, 4, 4);
                }
            }

            //Drawing statistics
            // 1. Calculate Stats
            int critterCount = _world.Entities.OfType<Critter>().Count();
            int foodCount = _world.Entities.OfType<FoodPellet>().Count();

            // 2. Format the string
            string stats = $"--- ECOSYSTEM STATS ---\n" +
                           $"Population: {critterCount}\n" +
                           $"Food Count: {foodCount}\n" +
                           $"Status: {(critterCount > 0 ? "Stable" : "EXTINCT")}";

            // 3. Draw it to the screen
            using (Font font = new Font("Consolas", 12, FontStyle.Bold))
            {
                // Draw a small background box for readability
                g.FillRectangle(new SolidBrush(Color.FromArgb(150, 0, 0, 0)), 10, 10, 220, 80);
                // Draw the text
                g.DrawString(stats, font, Brushes.White, 15, 15);
            }
        }
    }
}
