using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Environment;
using Ecosystem_Simulator.UI;
using System;

namespace Ecosystem_Simulator
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Starting Ecosystem Simulator...");
            Settings.EnsureFilesExist();
            World world = new World(Settings.WorldWidth, Settings.WorldHeight);
            world.Seed(Settings.InitialCritterNumber, Settings.InitialPredatorNumber, Settings.InitialFoodPelletNumber);

            // Letting the HeadlessRunner take complete control
            HeadlessRunner runner = new HeadlessRunner(world);
            runner.Start(); 
        }
    }
}

