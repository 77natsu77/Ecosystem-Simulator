using System.Collections.Generic;
using Ecosystem_Simulator.Core.delegates;
public interface IUpdatable : IEntity // For things that have logic (even static things like rotting food)
{
    event SpawnRequestDelegate OnSpawnRequested; // A "Signal" sent to the World
    void Update(double deltaTime, IEnumerable<IEntity> nearby);
}