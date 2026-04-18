using System.Collections.Generic;

namespace Ecosystem_Simulator.Core.Interfaces
{
    public interface IUpdatable
    {
        Vector2 Position { get; }
        Vector2 Velocity { get; }
        void Update(double deltaTime, List<IUpdatable> nearbyEntities);
    }
}
