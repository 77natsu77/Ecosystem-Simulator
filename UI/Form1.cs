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
            _world.Seed(50, 200); // no of critters, no of fooding

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
            using (Pen borderPen = new Pen(Color.Gray, 2f)) // 2 pixel thick line
            {
                g.DrawRectangle(borderPen, 0, 0, _world.Width, _world.Height);
            }
            foreach (var entity in _world.Entities)
            {
                var brush = entity is Critter ? Brushes.Red : Brushes.Green; //Draw critters as white, and food as green
                e.Graphics.FillEllipse(brush, entity.Position.X, entity.Position.Y, 5, 5);
            }
        }
    }
}
