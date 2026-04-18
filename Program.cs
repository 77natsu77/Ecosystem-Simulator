using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Interfaces;
using Ecosystem_Simulator.Core.Policies;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using System;
using System.Threading;

namespace Ecosystem_Simulator
{
    internal class Program
    {
        static void Main(string[] args)
        {

            World world = new World(Settings.WorldWidth,Settings.WorldHeight);

            IEnergyPolicy standardMetabolism = new StandardMetabolism();
            IGenome starterDNA = new DefaultGenome(); 


            Random rng = new Random();
            for (int i = 0; i < 10; i++)
            {
                Vector2 randomPos = new Vector2(rng.Next(0, 800), rng.Next(0, 600));
                world.Spawn(new Critter(randomPos, standardMetabolism, starterDNA));
            }

            while (true)
            {
                world.Tick(0.016);
                Thread.Sleep(16);
            }
        }
    }
}