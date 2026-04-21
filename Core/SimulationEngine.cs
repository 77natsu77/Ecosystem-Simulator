using Ecosystem_Simulator.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecosystem_Simulator.Core
{
    public class SimulationEngine
    {
        private readonly World _world;
        public SimulationState CurrentState { get; private set; }

        public event Action Updated;

        public SimulationEngine(World world)
        {
            _world = world;
        }

        public void Advance(double deltaTime)
        {
            if (! (CurrentState == SimulationState.Running)) return;

            _world.Tick(deltaTime);

            
            Updated?.Invoke();
        }

        public void Start() => CurrentState = SimulationState.Running;
        public void Stop() => CurrentState = SimulationState.Paused;
    }
}
