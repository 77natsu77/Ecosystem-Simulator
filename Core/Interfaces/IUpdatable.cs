using System.Collections.Generic;

public interface IUpdatable : IEntity // For things that have logic (even static things like rotting food)
{
    void Update(double deltaTime, List<IEntity> nearby);
}