// For things that have physical movement
using Ecosystem_Simulator.Core;

public interface IMovable : IUpdatable
{
    Vector2 Velocity { get; }
    void InvertVelocityX();
    void InvertVelocityY();
    void ForcePosition(Vector2 newPos);
}