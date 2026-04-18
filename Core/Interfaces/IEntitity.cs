using Ecosystem_Simulator.Core;

public interface IEntity
{
    Vector2 Position { get; }
    bool IsPendingRemoval { get; }
}