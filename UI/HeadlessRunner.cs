using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.Structs;
using Ecosystem_Simulator.Entities;
using Ecosystem_Simulator.Environment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Diagnostics;
 using Microsoft.AspNetCore.SignalR;

namespace Ecosystem_Simulator.UI
{
    public class HeadlessRunner
    {
        private World _world;

        private bool _isRunning = true;
        
        private IHubContext<WorldHub> _hub; // This is how we'll send data to the frontend in real-time using SignalR
        public HeadlessRunner(World world, IHubContext<WorldHub> hub)
        {
            _world = world;
            _hub = hub;
            _stats_manager = new StatisticsManager(world); // Now that we have the world, we can initialize the stats manager properly
        }


        public void Start()
        {
            Console.WriteLine("Simulation started. Open http://localhost:5000/ to view.");

            int delayMs = (int)(Settings.TickRate * 1000);
            if (delayMs <= 0) delayMs = 16;

            while (_isRunning)
            {
                try
                {
                    _world.Tick(Settings.TickRate);
                    ProcessFrame(); // fire-and-forget is ok for 20ish FPS
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CRASH DURING TICK: {ex.Message}");
                }

                Thread.Sleep(delayMs);
            }
        }

       
        private  async Task ProcessFrame() // This is where we gather all the data we want to send to the frontend every frame, and also where we handle the stats logging and saving to CSV/HTML. It's a bit of a "kitchen sink" method right now, but it works for our purposes. In the future, we might want to refactor it into smaller methods if it gets too unwieldy.
        {
            // First, we create a snapshot of the world state that we can use for rendering and analysis without affecting the simulation update loop. This also populates the exportEntities list with the data we need to send to the frontend.
            FrameData frameData = _world.CreateSnapshot(); // This creates a snapshot of the world state that we can use for rendering and analysis without affecting the simulation update loop. It also populates the exportEntities list with the data we need to send to the frontend.
            //  Serialize data and send the frame data to all connected clients via SignalR, which is much more efficient than writing to a file every frame and having the frontend read from it. The frontend can just listen for "frame" events from the SignalR hub and update the visualization accordingly, which should result in a much smoother experience overall.
            string jsonString = JsonSerializer.Serialize(frameData);
            await _hub.Clients.All.SendAsync("frame", frameData); 
        }  
}}