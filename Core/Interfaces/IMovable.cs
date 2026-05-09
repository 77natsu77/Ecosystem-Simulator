// For things that have physical movement
using Ecosystem_Simulator.Core.Structs;

namespace Ecosystem_Simulator.Core.Interfaces
{
    public interface IMovable : IUpdatable, ICollidable
{
    Vector2 Velocity { get; }
    void InvertVelocityX();
    void InvertVelocityY();
    void ForcePosition(Vector2 newPos);
}
}
