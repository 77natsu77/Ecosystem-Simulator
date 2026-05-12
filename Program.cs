using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Environment;
using Ecosystem_Simulator.UI;
using System;
 using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace Ecosystem_Simulator
{
    internal class Program
    {
        // this took a damn while to figure out, but by starting the web server in the background and using SignalR to send data to the frontend in real-time, we can avoid all the issues with file I/O and have a much smoother experience overall. The frontend can just listen for "frame" events from the SignalR hub and update the visualization accordingly, which is way more efficient than constantly reading/writing files.
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args); // This sets up a minimal web server using ASP.NET Core, which is perfect for our needs. It's lightweight and easy to use, and it has built-in support for SignalR, which we'll use for real-time communication with the frontend.
            
            builder.WebHost.UseUrls("http://0.0.0.0:5000");// forces the server to listen on all network interfaces, which is necessary for it to be accessible from the frontend running in the browser. By default, ASP.NET Core only listens on localhost, so we need to explicitly tell it to listen on all interfaces. This way, when the frontend tries to connect to the SignalR hub at "http://localhost:5000/worldHub", it will be able to reach the server and establish a connection successfully.
            // Register services
           builder.Services.AddSignalR()
       .AddJsonProtocol(options => {
           options.PayloadSerializerOptions.PropertyNamingPolicy = null; // This tells the JSON serializer to preserve the original property names (PascalCase) instead of converting them to camelCase, which is the default behavior. This way, when we send data from the backend to the frontend, the property names will match exactly what we have in our C# classes, and we won't have to worry about any naming mismatches or confusion on the frontend side.
       });
            builder.Services.AddSingleton<World>(sp =>
            {
                var w = new World(Settings.WorldWidth, Settings.WorldHeight);
                w.Seed(Settings.InitialCritterNumber,Settings.InitialSmartyNumber, Settings.InitialPredatorNumber, Settings.InitialFoodPelletNumber);
                return w;
            });

            var app = builder.Build();

            // Serve index.html from "wwwroot"
            app.UseDefaultFiles();  // loads index.html by default
            app.UseStaticFiles();   // serves html/js/css

            app.MapHub<WorldHub>("/worldHub"); // This sets up a SignalR hub at the "/worldHub" endpoint, which the frontend can connect to for real-time updates.

            Console.WriteLine("Ecosystem Engine Running...");

            // Get the world and hub context from the DI container and start the headless runner
            var world = app.Services.GetRequiredService<World>();
            var hubContext = app.Services.GetRequiredService<IHubContext<WorldHub>>();
            var runner = new HeadlessRunner(world, hubContext);

            // Run simulation in the background
            _ = Task.Run(() => runner.Start());

            await app.RunAsync();
        }
    }

   
    public class WorldHub : Hub { }
}

