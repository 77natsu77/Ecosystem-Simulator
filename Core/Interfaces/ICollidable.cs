namespace Ecosystem_Simulator.Core.Interfaces
{
    public interface ICollidable
    {
        void DetectCollisions(IEnumerable<IEntity> nearbyEntities);
        bool IsPendingRemoval { get; } // Ensure that collidable entities can be marked for removal after collision processing
    }
}