using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.Policies;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using Ecosystem_Simulator.UI;
using System;
using System.Windows.Forms;
using System.Threading;

namespace Ecosystem_Simulator
{
    internal class Program
    {
        [STAThread]
        static void Main()
        {
            World world = new World(Settings.WorldWidth, Settings.WorldHeight);
            world.Seed(Settings.InitialCritterNumber, Settings.InitialFoodPelletNumber);
            //SimulationEngine engine = new SimulationEngine(world);

            //Set up UI
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1(world);    
            Application.Run(form);
        }



    }
}