using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.Policies;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using Ecosystem_Simulator.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

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

            StatisticsManager dataSaver = new StatisticsManager();
            dataSaver.SaveWorldManual(world);

            // WORK DONE TODAY
            // Planning tests to be implemented, should be added tomorrow
            // Planning modifications to make code more efficienct
            // Drafted a json parser, should be done by tommorow
            // Tried visualizing data via python, failed due to shcool computer restrictions
            // Visualized data through interactive html successful, albiet with alot of stress
            // Lastly, played around with settings values to maximize birth rate, death rate and mutations, its quite fun :)
            // Should probably fix the soght raidus visual bug tommorow as well
        }



    }
}