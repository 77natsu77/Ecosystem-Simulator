using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.Policies;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using Ecosystem_Simulator.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Ecosystem_Simulator
{
    internal class Program
    {
        [STAThread]
        static void Main()
        {
            Settings.EnsureFilesExist();
            World world = new World(Settings.WorldWidth, Settings.WorldHeight);
            world.Seed(Settings.InitialCritterNumber, Settings.InitialPredatorNumber , Settings.InitialFoodPelletNumber);
            //SimulationEngine engine = new SimulationEngine(world);

            //Set up UI
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1(world);    
            Application.Run(form);

            StatisticsManager dataSaver = new StatisticsManager();
            dataSaver.SaveWorldManual(world);

            //WORK DONE TODAY
            // Continued working on the predator classes I did last night (visualization and stats)
            // Also added cannibal logic
            // Added headers to the html files
            // Postponed test implementations due to advent of predators
            // Started work on json parser
            // Fixed sight radius visual bug, though I am not sure if that was the best method
            // Need to configure settings in a way which allows for multiple generations so the stats can become more interesting
        }



    }
}