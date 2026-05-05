using Ecosystem_Simulator.Core;
using Ecosystem_Simulator.Core.delegates;

public interface IEntity
{
    Vector2 Position { get; }
    bool IsPendingRemoval { get; }
    int Id { get; } // Added Id property to IEntity interface for consistent access across all entities
    public event SpawnRequestDelegate OnSpawnRequested; // Added event to IEntity for spawn requests
}