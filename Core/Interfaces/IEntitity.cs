using Ecosystem_Simulator.Core.Structs;
using Ecosystem_Simulator.Core.delegates;

namespace Ecosystem_Simulator.Core.Interfaces
{
    public interface IEntity
    {
        Vector2 Position { get; }
        bool IsPendingRemoval { get; }
        int Id { get; } // Added Id property to IEntity interface for consistent access across all entities
        public event SpawnRequestDelegate OnSpawnRequested; // Added event to IEntity for spawn requests
    }
}